using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IMealFilterService _mealFilterService;
        private readonly ApplicationDbContext _context;

        public FilterController(IMealFilterService mealFilterService, ApplicationDbContext context)
        {
            _mealFilterService = mealFilterService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] MealFilter filter)
        {
            var meals= await _mealFilterService.Filter(filter);
            var maxprice=await _context.Meals.MaxAsync(m=>m.Price);
            return Ok(new { meals, maxprice });
        }
    }
}
