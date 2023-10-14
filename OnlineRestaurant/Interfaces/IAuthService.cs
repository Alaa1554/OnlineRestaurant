using OnlineRestaurant.Dtos;

namespace OnlineRestaurant.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModelDto> RegisterAsync(RegisterModelDto registermodel);
        Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<string> AddRoleAsync(AddRoleDto role);
        Task<AuthModelDto> UpdateImg(string token,IFormFile userimg);
        Task<string> DeleteAccountAsync(string token);
    }
}
