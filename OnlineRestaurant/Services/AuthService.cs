﻿using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;




namespace OnlineRestaurant.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageService _imgService;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memorycashe;
        private readonly TimeSpan _codeExpiration=TimeSpan.FromMinutes(15);

        public AuthService(UserManager<ApplicationUser> userManager, IEmailSender emailsender, IMemoryCache memoryCache, IMapper mapper, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IImageService imgService, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _imgService = imgService;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailsender;
            _memorycashe = memoryCache;
        }

        public async Task<string> RegisterAsync(RegisterModelDto registermodel)
        {
            if (registermodel.Password.Length < 6)
            {
                return  "الباسورد يجب ان يحتوي علي 6 حروف او ارقام علي الاقل" ;
            }
            if (await _userManager.FindByEmailAsync(registermodel.Email) is not null)
            {
                return "البريد الالكتروني موجود بالفعل" ;
            }
            if (await _userManager.FindByNameAsync(registermodel.UserName) is not null)
            {
                return  "اسم المستخدم موجود بالفعل" ;
            }
            var user = _mapper.Map<ApplicationUser>(registermodel);
            user.UserImgUrl =registermodel.UserImg==null?null: _imgService.Upload(registermodel.UserImg);
            if (!string.IsNullOrEmpty(user.Message))
            {
                return  user.Message ;
            }
            var result = await _userManager.CreateAsync(user, registermodel.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return errors;
            }
            
            await _userManager.AddToRoleAsync(user, "User");
            var verificationcode = GenerateRandomCode();
            _emailSender.SendEmail(user.Email, "Verification Code", $"Your verification code is {verificationcode}");
            _memorycashe.Set($"{user.Id} verification", verificationcode, _codeExpiration);
            return string.Empty;
        }
        public async Task<AuthModelDto> VerifyAccountAsync(VerifyAccountDto verifyaccount)
        {
            var user=await _userManager.FindByEmailAsync(verifyaccount.Email);
            if(user==null)
                return new AuthModelDto { Message= "لم يتم العثور علي اي مستخدم" };
            if (!_memorycashe.TryGetValue($"{user.Id} verification", out string cashedcode))
                return new AuthModelDto { Message = "لم يتم العثور علي رمز التحقق او تم انتهاء صلاحيته " };
            if (verifyaccount.VerificationCode != cashedcode)
                return new AuthModelDto { Message = "رمز التحقق الذي ادخلته غير صحيح" };
            user.EmailConfirmed = true;
            var jwtSecurityToken = await CreateJwtToken(user);
            var UserWishList = new WishList
            {
                UserId = user.Id
            };
            await _context.wishLists.AddAsync(UserWishList);
            await _context.SaveChangesAsync();
            var authmodel = new AuthModelDto
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                UserImgUrl =user.UserImgUrl==null?null:Path.Combine("https://localhost:7166","images", user.UserImgUrl),
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return authmodel;

        }
        public async Task<string> ForgetPassword(EmailDto emailDto)
        {
            var user = await _userManager.FindByEmailAsync(emailDto.Email);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"http://localhost:3000/reset-password/{code}";
             _emailSender.SendEmail(
                emailDto.Email,
                "Reset Password",
                $"Please reset your password by <a href={HtmlEncoder.Default.Encode(callbackUrl)}>clicking here</a>.");
            return string.Empty;
        }
        public async Task<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var bytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
            var code=Encoding.UTF8.GetString(bytes);
            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword",code);
            if (!result)
                return "حدثت مشكله اثناء التحقق يرجي المحاوله مره اخري";
            var resetPasswordResult=await _userManager.ResetPasswordAsync(user,code,resetPasswordDto.NewPassword);
            if (!resetPasswordResult.Succeeded)
                return "حدثت مشكله اثناء تغيير الباسورد يرجي المحاوله مره اخري";
            return string.Empty;
        }
        public async Task<string> ResendVerificationCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var verificationCode = GenerateRandomCode();
            _emailSender.SendEmail(user.Email, "Verification Code", $"Your verification code is {verificationCode}");
            _memorycashe.Set($"{user.Id} verification", verificationCode, _codeExpiration);
            return string.Empty;
        } 
        public async Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenrequest)
        {
            if (tokenrequest.Password.Length < 6)
            {
                return new AuthModelDto { Message = "الباسورد يجب ان يحتوي علي 6 حروف او ارقام علي الاقل" };
            }
            var authModel = new AuthModelDto();
            var user = await _userManager.FindByEmailAsync(tokenrequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, tokenrequest.Password)||!user.EmailConfirmed)
            {
                authModel.Message = "البريد الالكتروني او كلمه السر غير صحيحه";
                return authModel;
            }
            var jwtSecurityToken = await CreateJwtToken(user);
            var rolelist = await _userManager.GetRolesAsync(user);
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Roles = rolelist.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.UserImgUrl = user.UserImgUrl == null ? null : Path.Combine("https://localhost:7166", "images", user.UserImgUrl);
            authModel.FirstName = user.FirstName;
            authModel.LastName = user.LastName;


            return authModel;

        }
        public async Task<string> AddRoleAsync(AddRoleDto role)
        {
            var user = await _userManager.FindByIdAsync(role.UserId);
            if (user == null || await _roleManager.FindByNameAsync(role.Role) == null|| role.Role == "SuperAdmin")
            {
                return "Role or UserId is Invaild";

            }
            if (await _userManager.IsInRoleAsync(user, role.Role))
            {
                return "User Already Assigned to this Role";
            }
            var result = await _userManager.AddToRoleAsync(user, role.Role);
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }
        public async Task<string> RemoveRoleAsync(string userid)
        {
            var user =await _userManager.FindByIdAsync(userid);
            if (user == null)
                return "No User is Found";
            var userroles = await _userManager.GetRolesAsync(user);
            if (!userroles.Contains("Admin"))
                return "Role Already Removed";
            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModelDto> UpdateAccount(string token, UpdateAccountDto dto)
        {

            var userId = GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new AuthModelDto { Message = "لم يتم العثور علي اي مستخدم" };
            }

            user.UserImgUrl = dto.UserImg==null?user.UserImgUrl:user.UserImgUrl==null?_imgService.Upload(dto.UserImg):_imgService.Update(user.UserImgUrl, dto.UserImg);
                if (!string.IsNullOrEmpty(user.Message))
                {
                    return new AuthModelDto { Message = user.Message };
                }
            
            user.FirstName=dto.FirstName??user.FirstName;
            user.LastName=dto.LastName??user.LastName;
            user.UserName = dto.UserName ?? user.UserName;
            var result= await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return new AuthModelDto { Message = errors };
            }
            var jwtSecurityToken = await CreateJwtToken(user);
            var rolelist = await _userManager.GetRolesAsync(user);
            var authModel = new AuthModelDto();
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Roles = rolelist.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.UserImgUrl = user.UserImgUrl == null ? null : Path.Combine("https://localhost:7166", "images", user.UserImgUrl);
            authModel.FirstName = user.FirstName;
            authModel.LastName=user.LastName;
            return authModel;

        }

        public async Task<string> DeleteAccountAsync (string token)
        {

            var userid = GetUserId(token);
            if (string.IsNullOrEmpty(userid) || !await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var user= await _userManager.FindByIdAsync(userid);
            if (user.UserImgUrl != null)
                _imgService.Delete(user.UserImgUrl);
            var result= await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return  errors ;
            }
            return string.Empty ;
        }
        public async Task<string> UpdatePassword(string token,UpdatePasswordDto dto)
        {
            
            var userid=GetUserId(token);
            
            if (string.IsNullOrEmpty(userid)||!await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return  "لم يتم العثور علي اي مستخدم" ;
            }
            var user = await _userManager.FindByIdAsync(userid);
            if (!await _userManager.CheckPasswordAsync(user, dto.OldPassword)) 
            { 
                return  "كلمه السر غير صحيحه" ;
            }
           var result= await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return errors;
            }
            await _signInManager.RefreshSignInAsync(user);
            return string.Empty;
            
        }
        public async Task<string> DeleteUserImage(string token)
        {
           var userid= GetUserId(token);
            if(string.IsNullOrEmpty(userid) || !await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var user=await _userManager.FindByIdAsync(userid);
            if(user.UserImgUrl!=null)
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
                return $"{errors}" ;
            }
            return string.Empty;
        }
        public async Task<AuthModelDto> GmailRegisterAsync(GmailRegisterDto registermodel)
        {
            var user = await _userManager.FindByEmailAsync(registermodel.Email);
            if (user is not null)
            {
                
                    if (registermodel.FirstName != user.FirstName ||
                   registermodel.LastName != user.LastName ||
                    registermodel.UserName != user.UserName ||
                    registermodel.UserImgUrl != user.UserImgUrl)
                    {
                        user.FirstName = registermodel.FirstName != user.FirstName ? registermodel.FirstName??user.FirstName : user.FirstName;
                        user.LastName = registermodel.LastName != user.LastName ? registermodel.LastName??user.LastName : user.LastName;
                        user.UserName = registermodel.UserName != user.UserName ? registermodel.UserName??user.UserName : user.UserName;
                        user.UserImgUrl = registermodel.UserImgUrl != user.UserImgUrl ? registermodel.UserImgUrl??user.UserImgUrl : user.UserImgUrl;
                        await _userManager.UpdateAsync(user);

                    }
                
                
                
                var authModel = new AuthModelDto();
                var jwtSecurityToken = await CreateJwtToken(user);
                var rolelist = await _userManager.GetRolesAsync(user);
                authModel.Email = user.Email;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                authModel.IsAuthenticated = true;
                authModel.Roles = rolelist.ToList();
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.UserName = user.UserName;
                authModel.UserImgUrl = user.UserImgUrl;
                authModel.FirstName = user.FirstName;
                authModel.LastName = user.LastName;


                return authModel;
            }
            else 
            {
                user = _mapper.Map<ApplicationUser>(registermodel);
                
                await _userManager.CreateAsync(user);
               
            
                await _userManager.AddToRoleAsync(user, "User");
                var jwtSecurityToken = await CreateJwtToken(user);
                var UserWishList = new WishList
                {
                    UserId = user.Id
                };
                await _context.wishLists.AddAsync(UserWishList);
               await _context.SaveChangesAsync();
                var authmodel = new AuthModelDto
                {
                    Email = user.Email,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    UserName = user.UserName,
                    UserImgUrl = user.UserImgUrl,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    
                };

                return authmodel;
            }
            
            
        }

        public string GetUserId(string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var jwttoken = tokenhandler.ReadJwtToken(token) as JwtSecurityToken;
            var userid = jwttoken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            return userid;
        }
        public IEnumerable<UserView> SearchForUserByName(SearchForUserByName searchForUser)
        {
            var users = _context.Users.Where(c => c.UserName.Contains(searchForUser.UserName.ToLower().Trim())).Paginate(searchForUser.Page, searchForUser.Size).Select(u => new UserView
            {
                UserId = u.Id,
                UserImgUrl = u.UserImgUrl == null ? null : u.UserImgUrl.Contains("googleusercontent") ? u.UserImgUrl : Path.Combine("https://localhost:7166", "images", u.UserImgUrl),
                UserName = u.UserName

            }).ToList();
            if(!users.Any())
                return Enumerable.Empty<UserView>();
            var rolesviews = _context.UserRoles.GroupBy(c => c.UserId).Select(r => new RolesView { UserId = r.Key, Roles = r.Select(c => c.RoleId).ToList(), RoleName = null }).ToList();
            for (int i = 0; i < rolesviews.Count; i++)
            {
                if (rolesviews[i].Roles.Count == 1)
                {
                    rolesviews[i].RoleName = "User";
                }
                else
                {
                    rolesviews[i].RoleName = "Admin";
                }

            }
            foreach (var user in users)
            {
                var userroles = rolesviews.SingleOrDefault(r => r.UserId == user.UserId);
                if (userroles != null)
                    user.Role = userroles.RoleName;
            }
            return users;
        }
        public IEnumerable<UserView> GetAllUsersAsync(PaginateDto dto)
        {
            var rolesviews = _context.UserRoles.GroupBy(c=>c.UserId).Select(r => new RolesView { UserId=r.Key, Roles =r.Select(c=>c.RoleId).ToList(),RoleName=null}).ToList();
            for (int i =0; i<rolesviews.Count;i++ )
            {
                if (rolesviews[i].Roles.Count == 1)
                {
                    rolesviews[i].RoleName = "User";
                }
                else
                {
                    rolesviews[i].RoleName = "Admin";
                }
                    
                
            }
            var users = _userManager.Users.Paginate(dto.Page, dto.Size).Select(u=>new UserView
            {
                UserId=u.Id,
                UserImgUrl =u.UserImgUrl==null?null:u.UserImgUrl.Contains("googleusercontent")?u.UserImgUrl: Path.Combine("https://localhost:7166", "images", u.UserImgUrl),
                UserName=u.UserName
                
            }).ToList();
            foreach (var user in users)
            {
                var userroles =rolesviews.SingleOrDefault(r => r.UserId == user.UserId);
                if (userroles != null)
                    user.Role= userroles.RoleName;
            }
            return users;
        }

        private string GenerateRandomCode()
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string numbers = "0123456789";
            const int codeLength = 6;

            Random random = new Random();
            char[] code = new char[codeLength];

          
            code[0] = characters[random.Next(characters.Length)];

            
            code[1] = numbers[random.Next(numbers.Length)];

           
            for (int i = 2; i < codeLength; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }

            
            for (int i = 0; i < codeLength; i++)
            {
                int j = random.Next(i, codeLength);
                char temp = code[i];
                code[i] = code[j];
                code[j] = temp;
            }

            string randomCode = new string(code);

            return randomCode;
        }

    }
}

