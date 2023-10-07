using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class MealFilterService : IMealFilterService
    {
        private readonly ApplicationDbContext _context;

        public MealFilterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealView>> Filter(MealFilter filter)
        {
            var filterFactory = new FilterStrategyFactory(_context);
            var filterStrategies = filterFactory.GetFilterStrategy(filter);
            if (!filterStrategies.Any())
                return Enumerable.Empty<MealView>();

            IEnumerable<MealView> meals = null;
            foreach (var view in filterStrategies)
            {
                meals = meals?.Intersect(await view.ApplyFilter()) ?? await view.ApplyFilter();
            }

            var mealPaginate = Paginate(meals, filter.Page, filter.Size);
            var orderedMeals = Mealsorder(mealPaginate, filter.OrderMeal);
            return orderedMeals;
        }
        private IEnumerable<MealView> Paginate(IEnumerable<MealView> source, int page, int size) 
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (size <= 0)
            {
                size = 10;
            }

            var result = source.Skip((page - 1) * size).Take(size).ToList();

            return result;
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
