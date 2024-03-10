using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealService
    {
        Task<MealByNameView> GetMealByNameAsync(string name,string?token);
        Task<Meal> GetMealByIdAsync(int id);
        Task<Meal> GetMealByNameAsync(string name);
        Task<MealDto> CreateMeal(Meal mealDto);
        Task<MealView> UpdateMealAsync(Meal meal,UpdateMealDto dto);
        Task<MealDto> DeleteMeal(Meal meal);
    }
}
