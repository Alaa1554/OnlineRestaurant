using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Filters
{
    public class CategoryFilter:IFilterStrategy
    {
        private readonly string? _category;
        private readonly ApplicationDbContext _context;

        public CategoryFilter(string? Category, ApplicationDbContext context)
        {
            _category = Category;
            _context = context;
        }

        public IEnumerable<Meal> ApplyFilter()
        {
            var categories = _category.Split(',');
            IEnumerable<Meal> meals = Enumerable.Empty<Meal>();
            foreach (var category in categories)
            {
                meals = meals.Union(_context.Meals.Include(c => c.Category).Include(c=>c.MealReviews).Include(c => c.Chef).Where(m => m.Category.Name == category.Trim()).ToList());
            }
            return meals; 
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.Category);
        }
    }
}
