using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class MealNameFilter:IFilterStrategy
    {
        private readonly string? _name;
        private readonly ApplicationDbContext _context;
        private readonly MealFilter _filter;
        public MealNameFilter(string? name, ApplicationDbContext context, MealFilter filter)
        {
            _name = name;
            _context = context;
            _filter = filter;
        }

        public IEnumerable<MealView> ApplyFilter()
        { 
            var Meals =  _context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Name.Contains(_name.Trim())).Paginate(_filter.Page, _filter.Size).Select(
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
                        OldPrice= m.OldPrice==0.00m?null:m.OldPrice,
                        Rate = m.Rate,
                        NumOfRate = m.NumOfRate
                    }
                    ).ToList();
            return Meals;
        }

        public bool CanApply(MealFilter filter)
        {
            return !string.IsNullOrEmpty(filter.MealName);
        }
    }
}
