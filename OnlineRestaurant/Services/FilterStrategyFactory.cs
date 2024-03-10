using OnlineRestaurant.Data;
using OnlineRestaurant.Filters;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class FilterStrategyFactory
    {
        private readonly ApplicationDbContext _context;

        public FilterStrategyFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<IFilterStrategy> GetFilterStrategy(MealFilter filter)
        {
            var strategies = new List<IFilterStrategy>
            {
                new CategoryFilter(filter.Category,_context,filter),
                new ChefNameFilter(filter.ChefName,_context, filter),
                new PriceFilter(filter.FromPrice,filter.ToPrice,_context,filter),
                new MealNameFilter(filter.MealName,_context, filter),
                
            };
            return strategies.Where(strategy => strategy.CanApply(filter));
        }
    }
}
