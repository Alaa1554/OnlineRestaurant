using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineRestaurant.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterModelDto registermodel);
        Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<string> AddRoleAsync(AddRoleDto role);
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
        Task<AuthModelDto> GmailRegisterAsync(GmailRegisterDto registermodel);
        Task<string> RemoveRoleAsync(string userid);
        Task<AuthModelDto> VerifyAccountAsync(VerifyAccountDto verifyaccount);
        Task<string> ResendVerificationCode(string email);
        Task<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<string> ForgetPassword(EmailDto emailDto);
        string GetUserId(string token);
        Task<AuthModelDto> GetAuthModelDto(ApplicationUser user);
    }
}
