using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Filters
{
    public class MealNameFilter:IFilterStrategy
    {
        private readonly string? _name;
        private readonly ApplicationDbContext _context;
        public MealNameFilter(string? name, ApplicationDbContext context)
        {
            _name = name;
            _context = context;
        }

        public IEnumerable<Meal> ApplyFilter()
        { 
            return _context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Name.Contains(_name.Trim())).ToList();
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.MealName);
        }
    }
}
