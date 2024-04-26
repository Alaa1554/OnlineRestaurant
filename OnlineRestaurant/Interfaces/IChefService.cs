using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IChefService
    {
        IEnumerable<ChefView> GetChefs(PaginateDto dto);
        Task<ChefView> GetChefByIdAsync(int id);
        Task<ChefDto> CreateChef(Chef chef);
        Task<ChefDto> UpdateChefAsync(int id,UpdateChefDto dto);
        Task<ChefDto> DeleteChefAsync(int id);
        IEnumerable<ChefDto> GetChefsByCategoryIdAsync(int id, PaginateDto dto);
        

    }
}
