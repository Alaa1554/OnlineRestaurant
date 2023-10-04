using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineRestaurant.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
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
            var result= await _userManager.CreateAsync(user,registermodel.Password);
            if (!result.Succeeded)
            {
                var errors = "";
               foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return new AuthModelDto { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModelDto
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };
        }
        public async Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenrequest)
        {
            var authModel = new AuthModelDto();
            var user = await _userManager.FindByEmailAsync(tokenrequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user,tokenrequest.Password))
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

      
    }
}
