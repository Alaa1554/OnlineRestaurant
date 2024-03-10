using OnlineRestaurant.Dtos;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterModelDto registermodel);
        Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<string> AddRoleAsync(AddRoleDto role);
        Task<AuthModelDto> UpdateAccount(string token,UpdateAccountDto dto);
        Task<string> DeleteAccountAsync(string token);
        Task<string> UpdatePassword(string token, UpdatePasswordDto dto);
        string GetUserId(string token);
        Task<string> DeleteUserImage(string token);
        Task<AuthModelDto> GmailRegisterAsync(GmailRegisterDto registermodel);
        IEnumerable<UserView> GetAllUsersAsync(PaginateDto dto);
        Task<string> RemoveRoleAsync(string userid);
        Task<AuthModelDto> VerifyAccountAsync(VerifyAccountDto verifyaccount);
        Task<string> ResendVerificationCode(string email);
        IEnumerable<UserView> SearchForUserByName(SearchForUserByName searchForUser);
    }
}
