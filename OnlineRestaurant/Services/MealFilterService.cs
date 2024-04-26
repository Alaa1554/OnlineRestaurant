using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public MealFilterService(ApplicationDbContext context, IAuthService authService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<MealFilterView> Filter(string? token,MealFilter filter)
        {
            var filterFactory = new FilterStrategyFactory(_context);
            var filterStrategies = filterFactory.GetFilterStrategy(filter);
            IEnumerable<Meal> meals = null;
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

            IEnumerable<MealView> mealsView =_mapper.Map<IEnumerable<MealView>>(meals.Paginate(filter.Page, filter.Size));

            var orderedMeals = MealsOrder(mealsView, filter.OrderMeal);
            if (token != null) 
            {
                foreach (var meal in orderedMeals)
                {
                    bool isFavourite=false;

                    var userId = _authService.GetUserId(token);
                    if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
                        return new MealFilterView { NumOfPages = -1 };
                    var wishListId =await _context.wishLists.SingleOrDefaultAsync(v => v.UserId == userId);
                    if (await _context.WishListMeals.AnyAsync(w => w.MealId == meal.Id && w.WishListId == wishListId.Id))
                    {
                        isFavourite = true;
                    }
                    meal.IsFavourite = isFavourite;
                }
            }
            
            return new MealFilterView { Meals=orderedMeals,NumOfPages=numOfPages};
        }
      
        private IEnumerable<MealView> MealsOrder(IEnumerable<MealView> source,string? result)
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
