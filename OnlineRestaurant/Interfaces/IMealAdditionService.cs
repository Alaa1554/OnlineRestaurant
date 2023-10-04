using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealAdditionService
    {
        Task<IEnumerable<MealAddition>> GetMealAdditionsAsync(int id);
        Task<MealAddition> GetMealAdditionByIdAsync(int id);
        Task<MealAddition> CreateMealAddition(MealAddition Dto);
        Task<MealAddition> UpdateMealAdditionAsync(MealAddition mealAddition, UpdateMealAdditionDto dto);
        MealAddition DeleteMealAddition(MealAddition mealAddition);
    }
}
