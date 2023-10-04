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
            _context.SaveChanges();
            return addcategory;
        }

        public Category DeleteCategoryAsync(Category category)
        {
            _imgService.DeleteImg(category);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return category;
        }

        public async Task<IEnumerable<CategoryView>> GetCategoryAsync()
        {
            var meals =await _context.Categories.Select(c=>new CategoryView 
            { 
                Id=c.Id,
                Name=c.Name,
                CategoryImg=c.CategoryUrl,
                NumOfChefs=c.Chefs.Count(),
                NumOfMeals = c.Meals.Count()
            }).ToListAsync();
            return meals;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(b=>b.Id==id);
            if (category == null)
                return new Category { Message = $"No Category is found with Id :{id}" };
            return category;
        }

        public Category UpdateCategoryAsync(Category category, UpdateCategoryDto dto)
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
            _context.SaveChanges();
            return category;
        }
    }
}
