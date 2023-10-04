using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IMealFilterService _mealFilterService;

        public FilterController(IMealFilterService mealFilterService)
        {
            _mealFilterService = mealFilterService;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] MealFilter filter)
        {
            var meals= await _mealFilterService.Filter(filter);
            if (!meals.Any())
            {
                return BadRequest("No Filter is match With Your Filter");
            }
            return Ok(meals);
        }
    }
}
