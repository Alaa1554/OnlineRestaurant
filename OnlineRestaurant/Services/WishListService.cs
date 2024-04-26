using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Views;
using OnlineRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using AutoMapper;

namespace OnlineRestaurant.Services
{
    public class WishListService : IWishListService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public WishListService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAuthService authService, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<string> AddToWishList(string token, int mealid)
        {
            var userId =_authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if(!await _context.Meals.AnyAsync(m => m.Id == mealid))
            {
                return "لم يتم العثور علي اي وجبه";
            }
            var wishList= await  _context.wishLists.FirstOrDefaultAsync(c=>c.UserId == userId);
            if(await _context.WishListMeals.AnyAsync(c=>c.WishListId==wishList.Id&&c.MealId==mealid))
            {
                return "تمت اضافه الوجبه الي المفضله بالفعل";
            }
            var wishListMeal=new WishListMeal { MealId = mealid,WishListId=wishList.Id };
            await _context.WishListMeals.AddAsync(wishListMeal);
            await _context.SaveChangesAsync();
            return string.Empty;
        }


        public async Task<WishListMealView> GetWishlistAsync(string userid, PaginateDto dto)
        {
            if (!await _userManager.Users.AnyAsync(c => c.Id == userid))
            {
                return new WishListMealView { WishListId=-1} ;
            }
            var wishList= await _context.wishLists.SingleOrDefaultAsync(c=>c.UserId == userid);
            var wishListMealDto = _context.WishListMeals.Where(c => c.WishListId == wishList.Id).Include(c => c.Meals).ThenInclude(c => c.Additions).Include(c => c.Meals).ThenInclude(c => c.Category).Include(c => c.Meals).ThenInclude(c => c.Chef).Include(c => c.Meals).ThenInclude(c => c.MealReviews).Paginate(dto.Page, dto.Size);
            var wishListMealView=_mapper.Map<WishListMealView>(new WishListMealDto { WishListId=wishList.Id,Meals=wishListMealDto});
            wishListMealView.Meals.ToList().ForEach(m=>m.IsFavourite = true);
            return wishListMealView;
        }

        public async Task<string> RemoveFromWishList(string token, int mealid)
        {
            var userId =_authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return "لم يتم العثور علي اي مستخدم";
            }
            if(!await _context.Meals.AnyAsync(m=>m.Id == mealid))
            {
                return $"No Meal is Found With Id:{mealid}";
            }
            var wishList = await _context.wishLists.SingleOrDefaultAsync(c => c.UserId == userId);
            var wishListMeal = await _context.WishListMeals.SingleOrDefaultAsync(c => c.WishListId == wishList.Id&&c.MealId==mealid);
            if(wishListMeal == null)
            {
                return $"No Meal Is Found With Id:{mealid}! in This Wishlist";
            }
            _context.WishListMeals.Remove(wishListMeal);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
    }
}
