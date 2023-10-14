using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Protocol;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OnlineRestaurant.Services
{
    public class WishListService : IWishListService
    {
        private readonly ApplicationDbContext _context;

        public WishListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddToWishList(string token, int mealid)
        {
            var userid = GetUserId(token);
            var wishlist= await  _context.wishLists.FirstOrDefaultAsync(c=>c.UserId == userid);
            var WishListMeal=new WishListMeals { MealId = mealid,WishListId=wishlist.Id };
            await _context.WishListMeals.AddAsync(WishListMeal);
            _context.SaveChanges();
            return "تم اضافه الوجبه بنجاح الي المفضله";
             
        }

        public string GetUserId(string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var jwttoken = tokenhandler.ReadJwtToken(token) as JwtSecurityToken;
            var userid = jwttoken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            return userid;
        }

        public async Task<IEnumerable< WishListMealView>> GetWishlistAsync(string token)
        {
            var userid= GetUserId(token);
            var WishList= await _context.wishLists.SingleOrDefaultAsync(c=>c.UserId == userid);
          
            var WishListMeals=await _context.WishListMeals.Where(c => c.WishListId == WishList.Id).Include(c => c.Meals).GroupBy(c => c.WishListId).Select(c => new WishListMealView {WishListId=c.Key,Meals=c.Select(x=>x.Meals).ToList()}).ToListAsync();
            return WishListMeals;
        }

        public async Task<string> RemoveFromWishList(string token, int mealid)
        {
            var userid = GetUserId(token);
            var WishList = await _context.wishLists.SingleOrDefaultAsync(c => c.UserId == userid);
            var WishListMeal = await _context.WishListMeals.SingleOrDefaultAsync(c => c.WishListId == WishList.Id&&c.MealId==mealid);
            _context.WishListMeals.Remove(WishListMeal);
            _context.SaveChanges();
            return "تم حذف الوجبه بنجاح من المفضله";
        }
    }
}
