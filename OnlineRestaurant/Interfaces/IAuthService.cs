using OnlineRestaurant.Dtos;

namespace OnlineRestaurant.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModelDto> RegisterAsync(RegisterModelDto registermodel);
        Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<string> AddRoleAsync(AddRoleDto role);
        Task<AuthModelDto> UpdateAccount(string token,UpdateAccountDto dto);
        Task<string> DeleteAccountAsync(string token);
        Task<string> UpdatePassword(string token, UpdatePasswordDto dto);
        string GetUserId(string token);
        Task<string> DeleteUserImage(string token);
        Task<AuthModelDto> GmailRegisterAsync(GmailRegisterDto registermodel);
    }
}
