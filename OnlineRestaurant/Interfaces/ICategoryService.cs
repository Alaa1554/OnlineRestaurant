using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<CategoryView> GetCategory(PaginateDto dto);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategory(Category category);
        Task<CategoryDto> UpdateCategoryAsync(int id, Category dto);
        Task<CategoryDto> DeleteCategoryAsync(int id);
    }
}
