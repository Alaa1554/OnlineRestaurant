using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealService
    {
        Task<MealByNameView> GetMealByNameAsync(string name,string?token);
        Task<MealDto> CreateMeal(InsertMealDto mealDto);
        Task<MealDto> UpdateMealAsync(string name,InsertMealDto dto);
        Task<MealDto> DeleteMeal(string name);
    }
}
