using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class CategoryFilter:IFilterStrategy
    {
        private readonly string? _Category;
        private readonly ApplicationDbContext _context;
        public CategoryFilter(string? Category, ApplicationDbContext context)
        {
            _Category = Category;
            _context = context;
        }

        public async Task<IEnumerable<MealView>> ApplyFilter()
        {
            var Categories = _Category.Split(',');
            IEnumerable<Meal> Meals = Enumerable.Empty<Meal>();
            foreach (var Category in Categories)
            {
                Meals = Meals.Union(await _context.Meals.Include(c => c.Category).Include(c => c.Chef).Where(m => m.Category.Name == Category.Trim()).ToListAsync());
            }
            var MealsView = Meals.Select(
                    m => new MealView
                    {
                        Id = m.Id,
                        Categoryid = m.CategoryId,
                        Name = m.Name,
                        CategoryName = m.Category.Name,
                        ChefId = m.ChefId,
                        ChefName = m.Chef.Name,
                        MealImgUrl = m.MealImgUrl,
                        Price = m.Price
                    }
                    ).ToList();
            return MealsView;
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.Category);
        }
    }
}
