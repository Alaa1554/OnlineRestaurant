
using Microsoft.AspNetCore.Mvc;

using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;


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
            
            return Ok( meal);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateMealAsync([FromForm] Meal mealDto)
        {
            var Meal = await _mealService.CreateMeal(mealDto);
            if (!string.IsNullOrEmpty(Meal.Message)) 
            {
               return BadRequest(Meal.Message);
            }
            var Message =  "تم اضافه الوجبه بنجاح" ;
            return Ok(new { Meal, Message });
            

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMealAsync(int id, [FromForm] UpdateMealDto meal)
        {
            var getmeal = await _mealService.GetMealByIdAsync(id);

            if (!string.IsNullOrEmpty(getmeal.Message))
            {
                return NotFound(getmeal.Message);
            }

            var UpdatedData = await _mealService.UpdateMealAsync(getmeal, meal);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
               return BadRequest(UpdatedData.Message);
            }
            var Message = "تم تعديل الوجبه بنجاح";
            return Ok(new { UpdatedData, Message });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var meal =await _mealService.GetMealByIdAsync(id);
            if (!string.IsNullOrEmpty(meal.Message))
            {
               return NotFound(meal.Message);
            }
            var DeletedData =await _mealService.DeleteMeal(meal);
            var Message = "تم حذف الوجبه بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
