using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;


namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMealByNameAsync([FromQuery]string name, [FromHeader] string? token)
        {
            var meal =await _mealService.GetMealByNameAsync(name,token);
            if (!string.IsNullOrEmpty(meal.Message))
            {
                return NotFound(meal.Message);
            }
            return Ok(meal);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMealAsync([FromForm] InsertMealDto mealDto)
        {
            var meal = await _mealService.CreateMeal(mealDto);
            if (!string.IsNullOrEmpty(meal.Message)) 
            {
               return BadRequest(meal.Message);
            }
            var message =  "تم اضافه الوجبه بنجاح" ;
            return Ok(new { meal, message });
        }
        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMealAsync(string name, [FromForm] InsertMealDto dto)
        {
            var result = await _mealService.UpdateMealAsync(name, dto);
            if (!string.IsNullOrEmpty(result.Message))
            {
               return BadRequest(result.Message);
            }
            var message = "تم تعديل الوجبه بنجاح";
            return Ok(new { result, message });
        }
        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMeal(string name)
        {
            var deletedData =await _mealService.DeleteMeal(name);
            if (!string.IsNullOrEmpty(deletedData.Message))
            {
                return NotFound(deletedData.Message);
            }
            var message = "تم حذف الوجبه بنجاح";
            return Ok(new { deletedData, message });
        }
    }
}
