using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImgService<Category> _imgService;

        public CategoryService(ApplicationDbContext context, IImgService<Category> imgService)
        {
            _context = context;
            _imgService = imgService;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            var errormessages= ValidateHelper<Category>.Validate(category);
            if (!string.IsNullOrEmpty(errormessages))
                return new Category { Message = errormessages };
            
            var addcategory = new Category
            {
                Name = category.Name,
            };
            _imgService.SetImage(addcategory, category.CategoryImg);
            if (!string.IsNullOrEmpty(addcategory.Message) )
                return new Category { Message = addcategory.Message };
            await _context.Categories.AddAsync(addcategory);
            await _context.SaveChangesAsync();
            return addcategory;
        }

        public async Task<Category> DeleteCategoryAsync(Category category)
        {
            _imgService.DeleteImg(category);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<IEnumerable<CategoryView>> GetCategoryAsync(PaginateDto dto)
        {
            var meals =await _context.Categories.Include(c=>c.Chefs).Include(c=>c.Meals).Select(c=>new CategoryView 
            { 
                Id=c.Id,
                Name=c.Name,
                CategoryImg=c.CategoryUrl,
                NumOfChefs=c.Chefs.Count(),
                NumOfMeals = c.Meals.Count()
            }).ToListAsync();
            var result = meals.Paginate(dto.Page, dto.Size);
            return result;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(b=>b.Id==id);
            if (category == null)
                return new Category { Message = $"No Category is found with Id :{id}" };
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category, UpdateCategoryDto dto)
        {
            var errormessages = ValidateHelper<UpdateCategoryDto>.Validate(dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Category { Message = errormessages };
            }
           
            _imgService.UpdateImg(category, dto.CategoryImg);
            if (!string.IsNullOrEmpty(category.Message))
                return new Category { Message = category.Message };
            category.Name = dto.Name ?? category.Name;
            _context.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
