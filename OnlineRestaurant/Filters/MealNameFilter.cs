using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

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

        public async Task<IEnumerable<MealView>> ApplyFilter()
        { 
            var Meals = await _context.Meals.Include(c => c.Category).Include(c => c.Chef).Where(m => m.Name.Contains(_name.Trim())).Select(
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
                    ).ToListAsync();
            return Meals;
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.MealName);
        }
    }
}
