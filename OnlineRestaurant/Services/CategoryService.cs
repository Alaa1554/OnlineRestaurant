using AutoMapper;
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
        private readonly IImageService _imgService;
        private readonly IMapper _mapper;

        public CategoryService(ApplicationDbContext context, IImageService imgService, IMapper mapper)
        {
            _context = context;
            _imgService = imgService;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateCategory(Category category)
        {
            var errormessages= ValidateHelper<Category>.Validate(category);
            if (!string.IsNullOrEmpty(errormessages))
                return new CategoryDto { Message = errormessages };
            
            var addcategory = new Category
            {
                Name = category.Name,
            };
            addcategory.CategoryUrl = _imgService.Upload(category.CategoryImg);
            if (!string.IsNullOrEmpty(addcategory.Message) )
                return new CategoryDto { Message = addcategory.Message };
            await _context.Categories.AddAsync(addcategory);
            await _context.SaveChangesAsync();
            var categoryDto=_mapper.Map<CategoryDto>(addcategory);
            return categoryDto;
        }

        public async Task<CategoryDto> DeleteCategoryAsync(Category category)
        {
            _imgService.Delete(category.CategoryUrl);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public IEnumerable<CategoryView> GetCategory(PaginateDto dto)
        {
            var meals = _context.Categories.Include(c=>c.Chefs).Include(c=>c.Meals).Paginate(dto.Page, dto.Size).Select(c=>new CategoryView 
            { 
                Id=c.Id,
                Name=c.Name,
                CategoryImg=Path.Combine("https://localhost:7166", "images", c.CategoryUrl),
                NumOfChefs=c.Chefs.Count(),
                NumOfMeals = c.Meals.Count()
            }).ToList();
           
            return meals;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(b=>b.Id==id);
            if (category == null)
                return new Category { Message = $"No Category is found with Id :{id}" };
            return category;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Category category, UpdateCategoryDto dto)
        {
            var errormessages = ValidateHelper<UpdateCategoryDto>.Validate(dto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new CategoryDto { Message = errormessages };
            }
           
            category.CategoryUrl=dto.CategoryImg==null?category.CategoryUrl:_imgService.Update(category.CategoryUrl,dto.CategoryImg);
            if (!string.IsNullOrEmpty(category.Message))
                return new CategoryDto { Message = category.Message };
            category.Name = dto.Name ?? category.Name;
            _context.Update(category);
            await _context.SaveChangesAsync();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }
    }
}
