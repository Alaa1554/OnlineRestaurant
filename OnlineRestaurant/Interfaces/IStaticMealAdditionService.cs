using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IStaticMealAdditionService
    {
        Task<StaticMealAddition> GetMealAdditionByIdAsync(int id);
        Task<StaticMealAddition> CreateMealAddition(StaticMealAddition Dto);
        StaticMealAddition UpdateMealAdditionAsync(StaticMealAddition mealAddition, UpdateStaticMealAdditionDto dto);
        StaticMealAddition DeleteMealAddition(StaticMealAddition mealAddition);
    }
}
