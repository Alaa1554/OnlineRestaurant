using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Filters
{
    public class PriceFilter: IFilterStrategy
    {
        private readonly int? _fromprice;
        private readonly int? _toprice;
        private readonly ApplicationDbContext _context;
        public PriceFilter(int? Fromprice, int? Toprice, ApplicationDbContext context)
        {
            _fromprice = Fromprice;
            _toprice = Toprice;
            _context = context;
        }

        public IEnumerable<Meal> ApplyFilter()
        {
            return _context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Price >= _fromprice&&m.Price<=_toprice).ToList();
        }

        public bool CanApply(MealFilter filter)
        {
            return filter.FromPrice.HasValue&&filter.ToPrice.HasValue;
        }
    }
}
