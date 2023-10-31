
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
        private readonly IAuthService _authService;

        public FilterController(IMealFilterService mealFilterService, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _mealFilterService = mealFilterService;
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromHeader] string? token, [FromQuery] MealFilter filter)
        {

            if (token != null)
            {
                var userid = _authService.GetUserId(token);
                if(!_userManager.Users.Any(u=>u.Id == userid))
                    token = null;
            }
            var meals= await _mealFilterService.Filter(token,filter);
            var maxprice=await _context.Meals.MaxAsync(m=>m.Price);
            return Ok(new { meals, maxprice });
        }
    }
}
