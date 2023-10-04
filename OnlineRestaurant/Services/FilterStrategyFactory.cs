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

        public IFilterStrategy GetFilterStrategy(MealFilter filter)
        {
            var strategies = new List<IFilterStrategy>
            {
                new CategoryFilter(filter.Category,_context),
                new ChefNameFilter(filter.ChefName,_context),
                new PriceFilter(filter.FromPrice,filter.ToPrice,_context),
                new MealNameFilter(filter.MealName,_context),
                
            };
            return strategies.SingleOrDefault(strategy => strategy.CanApply(filter));
        }
    }
}
