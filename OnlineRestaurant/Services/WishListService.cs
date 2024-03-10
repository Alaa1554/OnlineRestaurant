using Microsoft.EntityFrameworkCore;

using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Views;

using OnlineRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;

namespace OnlineRestaurant.Services
{
    public class WishListService : IWishListService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public WishListService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<string> AddToWishList(string token, int mealid)
        {
            var userid =_authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            var wishlist= await  _context.wishLists.FirstOrDefaultAsync(c=>c.UserId == userid);
            if(await _context.WishListMeals.AnyAsync(c=>c.WishListId==wishlist.Id&&c.MealId==mealid))
            {
                return "تمت اضافه الوجبه الي المفضله بالفعل";
            }
            var WishListMeal=new WishListMeal { MealId = mealid,WishListId=wishlist.Id };
            await _context.WishListMeals.AddAsync(WishListMeal);
            await _context.SaveChangesAsync();
            return string.Empty;
             
        }

       

        public async Task<IEnumerable< WishListMealView>> GetWishlistAsync(string userid, PaginateDto dto)
        {
           
            
            var WishList= await _context.wishLists.SingleOrDefaultAsync(c=>c.UserId == userid);
            
            var WishListMeals= _context.WishListMeals.Where(c => c.WishListId == WishList.Id).Include(c => c.Meals).ThenInclude(c=>c.Additions).Include(c=>c.Meals).ThenInclude(c=>c.Category).Include(c=>c.Meals).ThenInclude(c=>c.Chef).Paginate(dto.Page, dto.Size).GroupBy(c => c.WishListId).Select(c => new WishListMealView {WishListId=c.Key,Meals=c.Select(x=>x.Meals).Select(c=>new MealView { 
                Id=c.Id,
                Categoryid=c.CategoryId,
                Name=c.Name,
                CategoryName=c.Category.Name,
                ChefId=c.ChefId,
                ChefName=c.Chef.Name,
                Description=c.Description,
                MealImgUrl=c.MealImgUrl,
                Price=c.Price,
                OldPrice=c.OldPrice,
                Rate= c.Rate,
                NumOfRate= c.NumOfRate
            }).ToList()});
            return WishListMeals;
        }

        public async Task<string> RemoveFromWishList(string token, int mealid)
        {
            var userid =_authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if(!await _context.Meals.AnyAsync(m=>m.Id == mealid))
            {
                return $"No Meal is Found With Id:{mealid}";
            }
            var WishList = await _context.wishLists.SingleOrDefaultAsync(c => c.UserId == userid);
            var WishListMeal = await _context.WishListMeals.SingleOrDefaultAsync(c => c.WishListId == WishList.Id&&c.MealId==mealid);
            if(WishListMeal == null)
            {
                return $"No Meal Is Found With Id:{mealid}! in This Wishlist";
            }
            _context.WishListMeals.Remove(WishListMeal);
           await _context.SaveChangesAsync();
            return string.Empty;
        }
    }
}
