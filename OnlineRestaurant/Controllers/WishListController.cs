﻿
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
            var userid=_authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return BadRequest( "No User is Found!" );
            }
            var WishListMeals=await _wishListService.GetWishlistAsync(userid,paginate);
            bool nextPage = false;
            if (WishListMeals.Count() > paginate.Size)
            {
                WishListMeals = WishListMeals.Take(WishListMeals.Count()-1);
                nextPage = true;
            }
            var numOfWishListMeals = await _context.WishListMeals.CountAsync(w=>w.WishListId==WishListMeals.First().WishListId);
            var numOfPages =(int) Math.Ceiling((decimal)numOfWishListMeals /paginate.Size);
            return Ok(new { WishListMeals = WishListMeals, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpPost("{mealid}")]
        public async Task<IActionResult> AddToWishListAsync([FromHeader] string token, int mealid)
        {
            var message=await _wishListService.AddToWishList(token, mealid);
            if(!string.IsNullOrEmpty(message))
                return BadRequest(message);
            return Ok("تم اضافه الوجبه بنجاح الي المفضله");
        }
        [HttpDelete("{mealid}")]
        public async Task<IActionResult> DeleteFromWishListAsync([FromHeader]string token,int mealid)
        {
            var message=await _wishListService.RemoveFromWishList(token, mealid);
            if (!string.IsNullOrEmpty(message))
                return BadRequest(message);
            return Ok("تم حذف الوجبه بنجاح من المفضله");
        }
    }
}
