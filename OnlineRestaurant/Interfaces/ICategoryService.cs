using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface ICategoryService
    {
     Task<IEnumerable<CategoryView>> GetCategoryAsync(PaginateDto dto);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategory(Category category);
       Task<Category> UpdateCategoryAsync(Category category, UpdateCategoryDto dto);
        Task<Category> DeleteCategoryAsync(Category category);
    }
}
