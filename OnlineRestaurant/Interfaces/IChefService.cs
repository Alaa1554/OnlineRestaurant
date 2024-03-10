using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IChefService
    {
        IEnumerable<ChefView> GetChefs(PaginateDto dto);
        Task<Chef> GetChefByIdAsync(int id);
        Task<ChefDto> CreateChef(Chef chef);
        Task<ChefDto> UpdateChefAsync(Chef chef,UpdateChefDto dto);
        Task<ChefDto> DeleteChefAsync(Chef chef);
        Task<IEnumerable<ChefDto>> GetChefsByCategoryIdAsync(int id, PaginateDto dto);
        

    }
}
