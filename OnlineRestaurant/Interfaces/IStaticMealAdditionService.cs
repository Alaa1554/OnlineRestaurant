using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IStaticMealAdditionService
    {
        Task<StaticMealAddition> GetMealAdditionByIdAsync(int id);
        Task<StaticMealAddition> CreateMealAddition(StaticMealAddition Dto);
        Task<StaticMealAddition> UpdateMealAdditionAsync(StaticMealAddition mealAddition, UpdateStaticMealAdditionDto dto);
        Task<StaticMealAddition> DeleteMealAddition(StaticMealAddition mealAddition);
        Task<IEnumerable<StaticMealAddition>> GetAllAdditions();
        
    }
}
