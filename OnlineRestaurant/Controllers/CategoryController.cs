using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {

            var categories = await _categoryService.GetCategoryAsync();
            return Ok(categories);
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

            var UpdatedData = _categoryService.UpdateCategoryAsync(getcategory, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            return Ok(UpdatedData);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChefAsync(int id)
        {

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (!string.IsNullOrEmpty(category.Message))
            {
                return NotFound(category.Message);
            }
            var DeletedData = _categoryService.DeleteCategoryAsync(category);
            return Ok(DeletedData);
        }
    }
}
