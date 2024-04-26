using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;


namespace OnlineRestaurant.Interfaces
{
    public interface IMealAdditionService
    {
        IEnumerable<MealAddition> GetMealAdditions(int id, PaginateDto dto);
        Task<MealAddition> CreateMealAddition(MealAddition Dto);
        Task<MealAddition> UpdateMealAdditionAsync(int id, UpdateMealAdditionDto dto);
        Task<MealAddition> DeleteMealAddition(int id);
        Task<string> DeleteChoiceAsync(int ChoiceId);
        Task<string> AddChoiceAsync(Choice choice);
    }
}
