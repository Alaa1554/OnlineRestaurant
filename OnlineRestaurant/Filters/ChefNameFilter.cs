using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class ChefNameFilter: IFilterStrategy
    {
        private readonly string? _ChefName;
        private readonly ApplicationDbContext _context;
        private readonly MealFilter _filter;
        public ChefNameFilter(string? ChefName, ApplicationDbContext context, MealFilter filter)
        {
            _ChefName = ChefName;
            _context = context;
            _filter = filter;
        }

        public  IEnumerable<MealView> ApplyFilter()
        {
            var Chefs= _ChefName.Split(',');
            IEnumerable<Meal>  Meals = Enumerable.Empty<Meal>(); 
            foreach (var Chef in Chefs)
            {
                Meals = Meals.Union(_context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Chef.Name == Chef.Trim()).Paginate(_filter.Page, _filter.Size).ToList());
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
                        MealImgUrl = Path.Combine("https://localhost:7166", "images", m.MealImgUrl),
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
