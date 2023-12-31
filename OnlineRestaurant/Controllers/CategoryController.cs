﻿
using Microsoft.AspNetCore.Mvc;
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

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {

            var categories = await _categoryService.GetCategoryAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var category=await _categoryService.GetCategoryByIdAsync(id);
            if(!string.IsNullOrEmpty(category.Message))
              return NotFound(category.Message);
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
