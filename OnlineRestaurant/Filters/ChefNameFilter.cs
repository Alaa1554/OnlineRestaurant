using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class ChefNameFilter: IFilterStrategy
    {
        private readonly string? _ChefName;
        private readonly ApplicationDbContext _context;
        public ChefNameFilter(string? ChefName, ApplicationDbContext context)
        {
            _ChefName = ChefName;
            _context = context;
        }

        public async Task<IEnumerable<MealView>> ApplyFilter()
        {
            var Chefs= _ChefName.Split(',');
            IEnumerable<Meal>  Meals = Enumerable.Empty<Meal>(); 
            foreach (var Chef in Chefs)
            {
                Meals = Meals.Union(await _context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Chef.Name == Chef.Trim()).ToListAsync());
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
                        Price = m.Price,
                        OldPrice = m.OldPrice==0.00m?null:m.OldPrice,
                        Rate=m.Rate,
                        NumOfRate = m.NumOfRate
                    }
                    ).ToList();
            return MealsView;
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.ChefName);
        }
    }
}
