using OnlineRestaurant.Dtos;

namespace OnlineRestaurant.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModelDto> RegisterAsync(RegisterModelDto registermodel);
        Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<string> AddRoleAsync(AddRoleDto role);
    }
}
