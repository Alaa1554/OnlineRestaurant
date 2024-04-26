using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager; 

        public FilterController(IMealFilterService mealFilterService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _mealFilterService = mealFilterService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromHeader] string? token, [FromQuery] MealFilter filter)
        {
            var result=await _mealFilterService.Filter(token,filter);
            if (result.NumOfPages == -1)
                return NotFound("no user is found");
            var meals=result.Meals;
            var numOfPages=result.NumOfPages;
            bool nextPage = false;
            if (meals.Count() > filter.Size)
            {
                meals = meals.Take(meals.Count() - 1);
                nextPage = true;
            }
            
            var maxPrice=await _context.Meals.Select(m=>m.Price).DefaultIfEmpty().MaxAsync();
            return Ok(new { meals, maxPrice,nextPage,numOfPages });
        }
    }
}
