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
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public WishListController(IWishListService wishListService, UserManager<ApplicationUser> userManager, IAuthService authService, ApplicationDbContext context)
        {
            _wishListService = wishListService;
            _userManager = userManager;
            _authService = authService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishListAsync([FromHeader] string token, [FromQuery] PaginateDto paginate) 
        {
            var userId=_authService.GetUserId(token);
            var wishListMeals=await _wishListService.GetWishlistAsync(userId,paginate);
            if (wishListMeals.WishListId==-1)
            {
                return NotFound("No User is Found!");
            }
            bool nextPage = false;
            if (wishListMeals.Meals.Count() > paginate.Size)
            {
                wishListMeals.Meals = wishListMeals.Meals.Take(wishListMeals.Meals.Count()-1);
                nextPage = true;
            }
            var wishList=await _context.wishLists.SingleOrDefaultAsync(w=>w.UserId == userId);
            var numOfWishListMeals = await _context.WishListMeals.CountAsync(w=>w.WishListId==wishList.Id);
            var numOfPages =(int) Math.Ceiling((decimal)numOfWishListMeals /paginate.Size);
            return Ok(new { WishListMeals = wishListMeals, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpPost("{mealid}")]
        public async Task<IActionResult> AddToWishListAsync([FromHeader] string token, int mealid)
        {
            var errorMessage = await _wishListService.AddToWishList(token, mealid);
            if(!string.IsNullOrEmpty(errorMessage))
                return BadRequest(errorMessage);
            return Ok("تم اضافه الوجبه بنجاح الي المفضله");
        }
        [HttpDelete("{mealid}")]
        public async Task<IActionResult> DeleteFromWishListAsync([FromHeader]string token,int mealid)
        {
            var errorMessage = await _wishListService.RemoveFromWishList(token, mealid);
            if (!string.IsNullOrEmpty(errorMessage))
                return NotFound(errorMessage);
            return Ok("تم حذف الوجبه بنجاح من المفضله");
        }
    }
}
