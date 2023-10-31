﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using OnlineRestaurant.Data;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;

namespace OnlineRestaurant.Services
{
    public class MealFilterService : IMealFilterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMealService _mealService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;


        public MealFilterService(ApplicationDbContext context, IMealService mealService, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _context = context;
            _mealService = mealService;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<IEnumerable<MealView>> Filter(string? token,MealFilter filter)
        {
            var filterFactory = new FilterStrategyFactory(_context);
            var filterStrategies = filterFactory.GetFilterStrategy(filter);
            
            IEnumerable<MealView> meals = null;
            if (filterStrategies.Any())
            {
                foreach (var view in filterStrategies)
                {
                    meals = meals?.Intersect(await view.ApplyFilter()) ?? await view.ApplyFilter();
                }
            }
            else
            {
                 meals = await _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).Include(m => m.MealReviews).Select(b => new MealView
                {
                    Id = b.Id,
                    ChefId = b.ChefId,
                    ChefName = b.Chef.Name,
                    MealImgUrl = b.MealImgUrl,
                    Name = b.Name,
                    Price = b.Price,
                    Categoryid = b.CategoryId,
                    CategoryName = b.Category.Name,
                    Rate = decimal.Round((b.MealReviews.Sum(b => b.Rate) /
               b.MealReviews.Where(b => b.Rate > 0).DefaultIfEmpty().Count()), 1),
                    NumOfRate = b.MealReviews.Count(c => c.Rate > 0),
                    OldPrice = b.OldPrice == 0.00m ? null : b.OldPrice,


                }).ToListAsync();
            }
            
            
            var mealPaginate = Paginate(meals, filter.Page, filter.Size);
            var orderedMeals = Mealsorder(mealPaginate, filter.OrderMeal);
            if (token != null) 
            {
                foreach (var meal in orderedMeals)
                {
                    string isfavourite;

                    var userid = _authService.GetUserId(token);

                    var wishlistid = _context.wishLists.SingleOrDefault(v => v.UserId == userid);
                    if (_context.WishListMeals.Any(w => w.MealId == meal.Id && w.WishListId == wishlistid.Id))
                    {
                        isfavourite = "true";
                    }
                    else
                    {
                        isfavourite = "false";
                    }

                    meal.IsFavourite = isfavourite;
                }
            }
            
            return orderedMeals;
        }
        private IEnumerable<MealView> Paginate(IEnumerable<MealView> source, int page, int size) 
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (size <= 0)
            {
                size = 10;
            }

            var result = source.Skip((page - 1) * size).Take(size).ToList();

            return result;
        }
        private IEnumerable<MealView> Mealsorder(IEnumerable<MealView> source,string? result)
        { 
            if(result == null)
                return source;
            if (!source.Any())
                return source;
            IEnumerable<MealView> res;
            switch (result.ToLower())
            {
                case "pd":
                 res=source.OrderByDescending(m=>m.Price).ToList();
                    break;
                case "pa":
                  res= source.OrderBy(m=>m.Price).ToList();
                    break;
                case "sd":
                    res = source.OrderByDescending(m=>m.NumOfRate).ToList();
                    break;
                case "rd":
                    res=source.OrderByDescending(m=>m.Rate).ToList();
                    break;
                    default:
                    res= source.ToList();
                    break;
            }
            return res;
        }
       
    }
}
