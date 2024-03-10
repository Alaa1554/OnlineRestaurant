
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class CategoryFilter:IFilterStrategy
    {
        private readonly string? _Category;
        private readonly ApplicationDbContext _context;
        private readonly MealFilter _filter;

        public CategoryFilter(string? Category, ApplicationDbContext context, MealFilter filter)
        {
            _Category = Category;
            _context = context;
            _filter = filter;
        }

        public IEnumerable<MealView> ApplyFilter()
        {
            var Categories = _Category.Split(',');
            IEnumerable<Meal> Meals = Enumerable.Empty<Meal>();
            foreach (var Category in Categories)
            {
                Meals = Meals.Union(_context.Meals.Include(c => c.Category).Include(c=>c.MealReviews).Include(c => c.Chef).Where(m => m.Category.Name == Category.Trim()).Paginate(_filter.Page, _filter.Size).ToList());
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
                        MealImgUrl =Path.Combine("https://localhost:7166", "images", m.MealImgUrl),
                        Price = m.Price,
                        OldPrice = m.OldPrice==0.00m?null:m.OldPrice,
                        Rate=m.Rate,
                        NumOfRate=m.NumOfRate,
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
