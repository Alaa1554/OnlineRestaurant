using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Filters
{
    public class ChefNameFilter: IFilterStrategy
    {
        private readonly string? _chefName;
        private readonly ApplicationDbContext _context;
        public ChefNameFilter(string? ChefName, ApplicationDbContext context)
        {
            _chefName = ChefName;
            _context = context;
        }

        public  IEnumerable<Meal> ApplyFilter()
        {
            var chefs= _chefName.Split(',');
            IEnumerable<Meal>  meals = Enumerable.Empty<Meal>(); 
            foreach (var chef in chefs)
            {
                meals = meals.Union(_context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Chef.Name == chef.Trim()).ToList());
            }
            
            return meals;
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.ChefName);
        }
    }
}
