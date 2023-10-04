using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface ICategoryService
    {
     Task<IEnumerable<CategoryView>> GetCategoryAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategory(Category category);
        Category UpdateCategoryAsync(Category category, UpdateCategoryDto dto);
        Category DeleteCategoryAsync(Category category);
    }
}
