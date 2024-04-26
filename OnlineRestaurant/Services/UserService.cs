using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineRestaurant.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imgService;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, IImageService imageService, IAuthService authService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _imgService = imageService;
            _authService = authService;
            _context = context;
        }

        public async Task<AuthModelDto> UpdateAccount(string token, UpdateAccountDto dto)
        {

            var userId =_authService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new AuthModelDto { Message = "لم يتم العثور علي اي مستخدم" };
            }

            user.UserImgUrl = dto.UserImg == null ? user.UserImgUrl : user.UserImgUrl == null ? _imgService.Upload(dto.UserImg) : _imgService.Update(user.UserImgUrl, dto.UserImg);
            if (!string.IsNullOrEmpty(user.Message))
            {
                return new AuthModelDto { Message = user.Message };
            }

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.UserName = dto.UserName ?? user.UserName;
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
            var jwtSecurityToken = await _authService.CreateJwtToken(user);
            var roleList = await _userManager.GetRolesAsync(user);
            var authModel = new AuthModelDto();
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Roles = roleList.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.UserImgUrl = user.UserImgUrl == null ? null : Path.Combine("https://localhost:7166", "images", user.UserImgUrl);
            authModel.FirstName = user.FirstName;
            authModel.LastName = user.LastName;
            return authModel;
        }
        public async Task<string> DeleteAccountAsync(string token)
        {

            var userId =_authService.GetUserId(token);
            if (string.IsNullOrEmpty(userId) || !await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var user = await _userManager.FindByIdAsync(userId);
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
            if (string.IsNullOrEmpty(userId) || !await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var user = await _userManager.FindByIdAsync(userId);
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
        public IEnumerable<UserView> SearchForUserByName(SearchForUserByName searchForUser)
        {
            var users = _context.Users.Where(c => c.UserName.Contains(searchForUser.UserName.ToLower().Trim())).Paginate(searchForUser.Page, searchForUser.Size).Select(u => new UserView
            {
                UserId = u.Id,
                UserImgUrl = u.UserImgUrl == null ? null : u.UserImgUrl.Contains("googleusercontent") ? u.UserImgUrl : Path.Combine("https://localhost:7166", "images", u.UserImgUrl),
                UserName = u.UserName

            }).ToList();
            if (!users.Any())
                return Enumerable.Empty<UserView>();
            var rolesViews = _context.UserRoles.GroupBy(c => c.UserId).Select(r => new RolesView { UserId = r.Key, Roles = r.Select(c => c.RoleId).ToList(), RoleName = null }).ToList();
            for (int i = 0; i < rolesViews.Count; i++)
            {
                if (rolesViews[i].Roles.Count == 1)
                {
                    rolesViews[i].RoleName = "User";
                }
                else
                {
                    rolesViews[i].RoleName = "Admin";
                }

            }
            foreach (var user in users)
            {
                var userRoles = rolesViews.SingleOrDefault(r => r.UserId == user.UserId);
                if (userRoles != null)
                    user.Role = userRoles.RoleName;
            }
            return users;
        }

        public IEnumerable<UserView> GetAllUsersAsync(PaginateDto dto)
        {
            var rolesViews = _context.UserRoles.GroupBy(c => c.UserId).Select(r => new RolesView { UserId = r.Key, Roles = r.Select(c => c.RoleId).ToList(), RoleName = null }).ToList();
            for (int i = 0; i < rolesViews.Count; i++)
            {
                if (rolesViews[i].Roles.Count == 1)
                {
                    rolesViews[i].RoleName = "User";
                }
                else
                {
                    rolesViews[i].RoleName = "Admin";
                }


            }
            var users = _userManager.Users.Paginate(dto.Page, dto.Size).Select(u => new UserView
            {
                UserId = u.Id,
                UserImgUrl = u.UserImgUrl == null ? null : u.UserImgUrl.Contains("googleusercontent") ? u.UserImgUrl : Path.Combine("https://localhost:7166", "images", u.UserImgUrl),
                UserName = u.UserName

            }).ToList();
            foreach (var user in users)
            {
                var userRoles = rolesViews.SingleOrDefault(r => r.UserId == user.UserId);
                if (userRoles != null)
                    user.Role = userRoles.RoleName;
            }
            return users;
        }
        public async Task<string> UpdatePassword(string token, UpdatePasswordDto dto)
        {
            var userId =_authService.GetUserId(token);

            if (string.IsNullOrEmpty(userId) || !await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var user = await _userManager.FindByIdAsync(userId);
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

    }
}
