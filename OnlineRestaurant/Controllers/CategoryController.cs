
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
            if(category==null)
              return NotFound($"No Category is found with Id :{id}");
            return Ok(category);
        }
        [HttpPost]

        public async Task<IActionResult> CreateCategoryAsync([FromForm] Category dto)
        {
            var category = await _categoryService.CreateCategory(dto);
            var message =  "تم اضافه التصنيف بنجاح" ;
            return Ok(new {category,message});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromForm] Category dto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result.Message);
            var message = "تم تعديل التصنيف بنجاح";
            return Ok(new { result, message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var deletedData = await _categoryService.DeleteCategoryAsync(id);
            if (!string.IsNullOrEmpty(deletedData.Message))
                return NotFound(deletedData.Message);
            var message = "تم حذف التصنيف بنجاح";
            return Ok(new { deletedData, message });
        }
    }
}
