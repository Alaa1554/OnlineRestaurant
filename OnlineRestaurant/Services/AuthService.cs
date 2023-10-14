using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineRestaurant.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImgService<ApplicationUser> _imgService;
        private readonly ApplicationDbContext _context;


        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IImgService<ApplicationUser> imgService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _imgService = imgService;
            _context = context;
        }

        public async Task<AuthModelDto> RegisterAsync(RegisterModelDto registermodel)
        {
            if (await _userManager.FindByEmailAsync(registermodel.Email) is not null)
            {
                return new AuthModelDto { Message = "Email Is Already Exist!" };
            }
            if (await _userManager.FindByNameAsync(registermodel.UserName) is not null)
            {
                return new AuthModelDto { Message = "UserName Is Already Exist!" };
            }
            var user = _mapper.Map<ApplicationUser>(registermodel);
            var result = await _userManager.CreateAsync(user, registermodel.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return new AuthModelDto { Message = errors };
            }
            var ImgErrors = _imgService.SetImage(user, registermodel.UserImg);
            if (!string.IsNullOrEmpty(ImgErrors))
            {
                return new AuthModelDto { Message = ImgErrors };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            var UserWishList=new WishList { 
                UserId=user.Id
            };
           await  _context.wishLists.AddAsync(UserWishList);
            _context.SaveChanges();
            var authmodel = new AuthModelDto
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                UserImgUrl = user.UserImgUrl,
                VerificationCode = GenerateRandomCode(),
            };

            return authmodel;
        }
        public async Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenrequest)
        {
            var authModel = new AuthModelDto();
            var user = await _userManager.FindByEmailAsync(tokenrequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, tokenrequest.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
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
            authModel.UserImgUrl = user.UserImgUrl;
            authModel.VerificationCode = null;


            return authModel;

        }
        public async Task<string> AddRoleAsync(AddRoleDto role)
        {
            var user = await _userManager.FindByIdAsync(role.UserId);
            if (user == null || await _roleManager.FindByNameAsync(role.Role) == null)
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
        public async Task<AuthModelDto> UpdateImg(string token, IFormFile userimg)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new AuthModelDto { Message = "No User is Found!" };
            }
            _imgService.UpdateImg(user, userimg);
            if (!string.IsNullOrEmpty(user.Message))
            {
                return new AuthModelDto { Message = user.Message };
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
            authModel.UserImgUrl = user.UserImgUrl;
            authModel.VerificationCode = null;
            return authModel;

        }

        public async Task<string> DeleteAccountAsync (string token)
        {
            var tokenhandler=new JwtSecurityTokenHandler();
            var jwttoken=tokenhandler.ReadJwtToken(token) as JwtSecurityToken;
            var userid = jwttoken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            if(string.IsNullOrEmpty(userid) )
            {
                return  "No User is Found!" ;
            }
            var user= await _userManager.FindByIdAsync(userid);
             if(user == null)
            {
                return  "No User is Found!" ;
            }
            _imgService.DeleteImg(user);
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

