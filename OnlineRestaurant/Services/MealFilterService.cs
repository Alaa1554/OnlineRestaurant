
using Microsoft.EntityFrameworkCore;

using OnlineRestaurant.Data;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class MealFilterService : IMealFilterService
    {
        private readonly ApplicationDbContext _context;
       
        private readonly IAuthService _authService;


        public MealFilterService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<IEnumerable<MealView>> Filter(string? token,MealFilter filter)
        {
            var filterFactory = new FilterStrategyFactory(_context);
            var filterStrategies = filterFactory.GetFilterStrategy(filter);
            
            IEnumerable<MealView> meals = null;
            if (filterStrategies.Any())
            {
                foreach (var view in filterStrategies)
                {
                    meals = meals?.Intersect(await view.ApplyFilter()) ?? await view.ApplyFilter();
                }
            }
            else
            {
                 meals = await _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).Select(b => new MealView
                {
                    Id = b.Id,
                    ChefId = b.ChefId,
                    ChefName = b.Chef.Name,
                    MealImgUrl = b.MealImgUrl,
                    Name = b.Name,
                    Price = b.Price,
                    Categoryid = b.CategoryId,
                    CategoryName = b.Category.Name,
                    Rate = b.Rate,
                    NumOfRate = b.NumOfRate,
                    OldPrice=b.OldPrice,


                }).ToListAsync();
            }
            
            
            var mealPaginate =meals.Paginate( filter.Page, filter.Size);
            var orderedMeals = Mealsorder(mealPaginate, filter.OrderMeal);
            if (token != null) 
            {
                foreach (var meal in orderedMeals)
                {
                    bool isfavourite=false;

                    var userid = _authService.GetUserId(token);

                    var wishlistid = await _context.wishLists.SingleOrDefaultAsync(v => v.UserId == userid);
                    if (await _context.WishListMeals.AnyAsync(w => w.MealId == meal.Id && w.WishListId == wishlistid.Id))
                    {
                        isfavourite = true;
                    }
                    else
                    {
                        isfavourite = false;
                    }

                    meal.IsFavourite = isfavourite;
                }
            }
            
            return orderedMeals;
        }
      
        private IEnumerable<MealView> Mealsorder(IEnumerable<MealView> source,string? result)
        { 
            if(result == null)
                return source;
            if (!source.Any())
                return source;
            IEnumerable<MealView> res;
            switch (result.ToLower())
            {
                case "pd":
                 res=source.OrderByDescending(m=>m.Price).ToList();
                    break;
                case "pa":
                  res= source.OrderBy(m=>m.Price).ToList();
                    break;
                case "sd":
                    res = source.OrderByDescending(m=>m.NumOfRate).ToList();
                    break;
                case "rd":
                    res=source.OrderByDescending(m=>m.Rate).ToList();
                    break;
                    default:
                    res= source.ToList();
                    break;
            }
            return res;
        }
       
    }
}
