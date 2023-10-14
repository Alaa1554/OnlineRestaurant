using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Interfaces;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpGet]
       public async Task<IActionResult> GetWishListAsync([FromHeader] string token ) 
        {
            var WishListMeals=await _wishListService.GetWishlistAsync(token);
            return Ok(WishListMeals);
        }
        [HttpPost("{mealid}")]
        public async Task<IActionResult> AddToWishListAsync([FromHeader] string token, int mealid)
        {
            var message=await _wishListService.AddToWishList(token, mealid);
            return Ok(message);
        }
        [HttpDelete("{mealid}")]
        public async Task<IActionResult> DeleteFromWishListAsync([FromHeader]string token,int mealid)
        {
            var message=await _wishListService.RemoveFromWishList(token, mealid);
            return Ok(message);
        }
    }
}
