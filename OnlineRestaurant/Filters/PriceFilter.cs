using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Filters
{
    public class PriceFilter: IFilterStrategy
    {
        private readonly int? _Fromprice;
        private readonly int? _Toprice;
        private readonly ApplicationDbContext _context;
        public PriceFilter(int? Fromprice,int? Toprice, ApplicationDbContext context)
        {
            _Fromprice= Fromprice;
            _Toprice= Toprice;
            _context = context;
        }

        public async Task<IEnumerable<MealView>> ApplyFilter()
        {
            var Meals = await _context.Meals.Include(c => c.Category).Include(c => c.Chef).Include(c=>c.MealReviews).Where(m => m.Price >= _Fromprice&&m.Price<=_Toprice).Select(
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
                        OldPrice= m.OldPrice==0.00m?null:m.OldPrice,
                        Rate = decimal.Round(m.MealReviews.Sum(c => c.Rate) / m.MealReviews.Where(c => c.Rate > 0).DefaultIfEmpty().Count(), 1),
                        NumOfRate = m.MealReviews.Count(r => r.Rate > 0)
                    }
                    ).ToListAsync();
            return Meals;
        }

        public bool CanApply(MealFilter filter)
        {
            return filter.FromPrice.HasValue&&filter.ToPrice.HasValue;
        }
    }
}
