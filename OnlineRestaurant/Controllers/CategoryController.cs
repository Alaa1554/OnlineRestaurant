
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;


namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ApplicationDbContext _context;

        public CategoryController(ICategoryService categoryService, ApplicationDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] PaginateDto paginate)
        {

            var categories =  _categoryService.GetCategory(paginate);
            bool nextPage = false;
            if (categories.Count() > paginate.Size)
            {
                categories = categories.Take(categories.Count() - 1);
                nextPage = true;
            }
            var numOfCategories = await _context.Categories.CountAsync();
            var numOfPages = (int)Math.Ceiling((decimal)numOfCategories / paginate.Size);
            return Ok(new { Categories = categories, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var category=await _categoryService.GetCategoryByIdAsync(id);
            if(!string.IsNullOrEmpty(category.Message))
              return NotFound(category.Message);
            category.CategoryUrl = Path.Combine("https://localhost:7166", "images", category.CategoryUrl);
            return Ok(category);
        }
        [HttpPost]

        public async Task<IActionResult> CreateCategoryAsync([FromForm] Category dto)
        {

            var Category = await _categoryService.CreateCategory(dto);
            if (!string.IsNullOrEmpty(Category.Message))
            {
                return BadRequest(Category.Message);
            }
            var Message =  "تم اضافه التصنيف بنجاح" ;
            return Ok(new {Category,Message});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromForm] UpdateCategoryDto dto)
        {
            var getcategory = await _categoryService.GetCategoryByIdAsync(id);

            if (!string.IsNullOrEmpty(getcategory.Message))
            {
                return NotFound(getcategory.Message);
            }

            var UpdatedData = await _categoryService.UpdateCategoryAsync(getcategory, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            var Message = "تم تعديل التصنيف بنجاح";
            return Ok(new { UpdatedData, Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (!string.IsNullOrEmpty(category.Message))
            {
                return NotFound(category.Message);
            }
            var DeletedData = await _categoryService.DeleteCategoryAsync(category);
            var Message = "تم حذف التصنيف بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
