using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IChefService
    {
        Task<IEnumerable<ChefView>> GetChefsAsync();
        Task<Chef> GetChefByIdAsync(int id);
        Task<Chef> CreateChef(Chef chef);
        Task<Chef> UpdateChefAsync(Chef chef,UpdateChefDto dto);
        Task<Chef> DeleteChefAsync(Chef chef);
        Task<IEnumerable<Chef>> GetChefsByCategoryIdAsync(int id);
        

    }
}
