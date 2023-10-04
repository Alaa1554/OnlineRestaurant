using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface IMealService
    {
        Task<IEnumerable<MealView>> GetMealsAsync();
        Task<Meal> GetMealByIdAsync(int id);
        Task<Meal> CreateMeal(Meal mealDto);
        Task<MealView> UpdateMealAsync(Meal meal,UpdateMealDto dto);
        Meal DeleteMeal(Meal meal);
    }
}
