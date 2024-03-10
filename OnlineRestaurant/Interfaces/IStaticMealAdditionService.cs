using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IStaticMealAdditionService
    {
        Task<StaticMealAddition> GetMealAdditionByIdAsync(int id);
        Task<StaticMealAdditionView> CreateMealAddition(StaticMealAddition Dto);
        Task<StaticMealAdditionView> UpdateMealAdditionAsync(StaticMealAddition mealAddition, UpdateStaticMealAdditionDto dto);
        Task<StaticMealAdditionView> DeleteMealAddition(StaticMealAddition mealAddition);
        IEnumerable<StaticMealAdditionView> GetAllAdditions(PaginateDto dto);
        
    }
}
