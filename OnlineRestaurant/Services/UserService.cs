using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imgService;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IImageService imageService, IAuthService authService, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _imgService = imageService;
            _authService = authService;
            _context = context;
            _mapper = mapper;
        }
        public async Task<AuthModelDto> UpdateAccount(string token, UpdateAccountDto dto)
        {

            var userId =_authService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return new AuthModelDto { Message = "لم يتم العثور علي اي مستخدم" };
            }
            if (await _userManager.FindByNameAsync(dto.UserName) != null&&user.UserName!=dto.UserName)
            {
                return new AuthModelDto { Message = "اسم المستخدم موجود بالفعل" };
            }
            _mapper.Map(dto,user);
            user.UserImgUrl = dto.UserImg == null ? user.UserImgUrl : user.UserImgUrl == null ? _imgService.Upload(dto.UserImg) : _imgService.Update(user.UserImgUrl, dto.UserImg);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return new AuthModelDto { Message = errors };
            }
          return await _authService.GetAuthModelDto(user);
        }
        public async Task<string> DeleteAccountAsync(string token)
        {
            var userId =_authService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if (user.UserImgUrl != null)
                _imgService.Delete(user.UserImgUrl);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return errors;
            }
            return string.Empty;
        }
        public async Task<string> DeleteUserImage(string token)
        {
            var userId =_authService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if (user.UserImgUrl != null)
                _imgService.Delete(user.UserImgUrl);
            user.UserImgUrl = null;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return $"{errors}";
            }
            return string.Empty;
        }
        public async Task<IEnumerable<UserView>> SearchForUserByName(SearchForUserByName searchForUser)
        {
            var users = _context.Users.Where(c => c.UserName.ToLower().Trim().Contains(searchForUser.UserName.ToLower().Trim())).Paginate(searchForUser.Page, searchForUser.Size);
            if (!users.Any())
                return Enumerable.Empty<UserView>();
            return await GetUserView(users); 
        }

        public async Task<IEnumerable<UserView>> GetAllUsersAsync(PaginateDto dto)
        {
            var users = _context.Users.Paginate(dto.Page, dto.Size);
            return await GetUserView(users);
        }
        public async Task<string> UpdatePassword(string token, UpdatePasswordDto dto)
        {
            var userId =_authService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if (!await _userManager.CheckPasswordAsync(user, dto.OldPassword))
            {
                return "كلمه السر غير صحيحه";
            }
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return errors;
            }
            return string.Empty;

        }
        private async Task<IEnumerable<UserView>> GetUserView(IEnumerable<ApplicationUser> users)
        {
            var userViews = new List<UserView>();
            var userIds = users.Select(u => u.Id).ToList();
            var roles =await _context.UserRoles.Where(ur=>userIds.Contains(ur.UserId)).ToListAsync();
            foreach (var user in users)
            {
                var role = roles.Where(ur=>ur.UserId==user.Id);
                var userView = _mapper.Map<UserView>(user);
                userView.Role = role.Count() == 1 ? "User" : "Admin";
                userViews.Add(userView);
            }
            return userViews;
        }

    }
}
