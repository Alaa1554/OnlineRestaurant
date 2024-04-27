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
            category.CategoryUrl = _imgService.Upload(category.CategoryImg);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            var categoryDto=_mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public async Task<CategoryDto> DeleteCategoryAsync(int id)
        {
            var category= await _context.Categories.SingleOrDefaultAsync(b => b.Id == id);
            if (category == null)
                return new CategoryDto { Message = $"No Category is found with Id :{id}" };
            _imgService.Delete(category.CategoryUrl);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public IEnumerable<CategoryView> GetCategory(PaginateDto dto)
        {
            return _mapper.Map<IEnumerable<CategoryView>>(_context.Categories.Include(c => c.Chefs).Include(c => c.Meals).Paginate(dto.Page, dto.Size));
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            return _mapper.Map<CategoryDto>(await _context.Categories.SingleOrDefaultAsync(b => b.Id == id));
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, Category dto)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(b => b.Id == id);
            if (category == null)
                return new CategoryDto { Message = $"No Category is found with Id :{id}" };
            category.CategoryUrl=dto.CategoryImg==null?category.CategoryUrl:_imgService.Update(category.CategoryUrl,dto.CategoryImg);
            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }
    }
}
