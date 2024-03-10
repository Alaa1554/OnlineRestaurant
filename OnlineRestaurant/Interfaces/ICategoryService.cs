using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface ICategoryService
    {
       IEnumerable<CategoryView> GetCategory(PaginateDto dto);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategory(Category category);
       Task<CategoryDto> UpdateCategoryAsync(Category category, UpdateCategoryDto dto);
        Task<CategoryDto> DeleteCategoryAsync(Category category);
    }
}
