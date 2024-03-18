
using Humanizer;
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

        public async Task<MealFilterView> Filter(string? token,MealFilter filter)
        {
            var filterFactory = new FilterStrategyFactory(_context);
            var filterStrategies = filterFactory.GetFilterStrategy(filter);
            
            IEnumerable<Meal> meals = null;
            IEnumerable<MealView> mealsView= null;
            int numOfPages = 0;
            int numOfMeals= 0;
            if (filterStrategies.Any())
            {
                foreach (var view in filterStrategies)
                {
                    meals = meals?.Intersect(view.ApplyFilter()) ?? view.ApplyFilter();
                }
                numOfMeals= meals.Count();
                numOfPages= (int)Math.Ceiling((decimal)numOfMeals / filter.Size);
            }
            else
            {
                meals = _context.Meals.Include(meal => meal.Chef).Include(b => b.Category);
                numOfMeals = meals.Count();
                numOfPages = (int)Math.Ceiling((decimal)numOfMeals / filter.Size);
            }

            mealsView = meals.Paginate(filter.Page, filter.Size).Select(b => new MealView
            {
                Id = b.Id,
                ChefId = b.ChefId,
                ChefName = b.Chef.Name,
                MealImgUrl = Path.Combine("https://localhost:7166", "images", b.MealImgUrl),
                Name = b.Name,
                Price = b.Price,
                Categoryid = b.CategoryId,
                CategoryName = b.Category.Name,
                Rate = b.Rate,
                NumOfRate = b.NumOfRate,
                OldPrice = b.OldPrice,


            }).ToList();

            var orderedMeals = Mealsorder(mealsView, filter.OrderMeal);
            if (token != null) 
            {
                foreach (var meal in orderedMeals)
                {
                    bool isfavourite=false;

                    var userid = _authService.GetUserId(token);

                    var wishlistid =await _context.wishLists.SingleOrDefaultAsync(v => v.UserId == userid);
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
            
            return new MealFilterView { Meals=orderedMeals,NumOfPages=numOfPages};
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
