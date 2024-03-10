using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;


namespace OnlineRestaurant.Interfaces
{
    public interface IMealAdditionService
    {
        IEnumerable<MealAddition> GetMealAdditions(int id, PaginateDto dto);
        Task<MealAddition> GetMealAdditionByIdAsync(int id);
        Task<MealAddition> CreateMealAddition(MealAddition Dto);
        Task<MealAddition> UpdateMealAdditionAsync(MealAddition mealAddition, UpdateMealAdditionDto dto,int? id);
        Task<MealAddition> DeleteMealAddition(MealAddition mealAddition);
        Task<string> DeleteChoiceAsync(int AdditionId, int ChoiceId);
        Task<string> AddChoiceAsync(int AdditionId, Choice choice);
    }
}
