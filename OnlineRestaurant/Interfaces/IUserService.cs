using OnlineRestaurant.Dtos;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IUserService
    {
        Task<AuthModelDto> UpdateAccount(string token, UpdateAccountDto dto);
        Task<string> DeleteAccountAsync(string token);
        Task<string> DeleteUserImage(string token);
        IEnumerable<UserView> SearchForUserByName(SearchForUserByName searchForUser);
        IEnumerable<UserView> GetAllUsersAsync(PaginateDto dto);
        Task<string> UpdatePassword(string token, UpdatePasswordDto dto);

    }
}
