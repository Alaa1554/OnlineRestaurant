using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IStaticMealAdditionService
    {
        Task<StaticMealAdditionView> CreateMealAddition(StaticMealAddition Dto);
        Task<StaticMealAdditionView> UpdateMealAdditionAsync(int id, UpdateStaticMealAdditionDto dto);
        Task<StaticMealAdditionView> DeleteMealAddition(int id);
        IEnumerable<StaticMealAdditionView> GetAllAdditions(PaginateDto dto);
        Task<StaticMealAdditionView> GetMealAdditionByIdAsync(int id);
    }
}
