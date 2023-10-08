using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private IMealService _mealService;
        private ApplicationDbContext _context;

        public MealController(IMealService mealService, ApplicationDbContext context)
        {
            _mealService = mealService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMealAsync()
        {
            var meals =await _mealService.GetMealsAsync();
            var maxprice = await _context.Meals.MaxAsync(m => m.Price);
            return Ok(new { meals, maxprice });
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
            return Ok(UpdatedData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var meal =await _mealService.GetMealByIdAsync(id);
            if (!string.IsNullOrEmpty(meal.Message))
            {
               return NotFound(meal.Message);
            }
            var DeletedData = _mealService.DeleteMeal(meal);
            return Ok(DeletedData);
        }
    }
}
