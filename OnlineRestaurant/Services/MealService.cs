using AutoMapper;
using Humanizer;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineRestaurant.Services
{
    public class MealService : IMealService
    {
        private readonly ApplicationDbContext _context;
        
        private readonly IImgService<Meal> _imgService;
        private readonly IWishListService _wishlist;
        private readonly IMapper _mapper;

        public MealService(ApplicationDbContext context, IImgService<Meal> imgService, IWishListService wishlist, IMapper mapper)
        {
            _context = context;
            _imgService = imgService;
            _wishlist = wishlist;
            _mapper = mapper;
        }
        public async Task<Meal> CreateMeal(Meal mealDto)
        {
            var errormessages=ValidateHelper<Meal>.Validate(mealDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Meal { Message = errormessages };
            }
            if (!await _context.Chefs.AnyAsync(chef=>chef.Id==mealDto.ChefId))
                return new Meal { Message = $"There is no Chef with Id : {mealDto.ChefId}!"};
            if (!await _context.Categories.AnyAsync(b=>b.Id==mealDto.CategoryId))
                return new Meal { Message = $"There is no Category with Id : {mealDto.CategoryId}!" };

            var meal = new Meal 
            { 
                Name =mealDto.Name,
                ChefId = mealDto.ChefId,
                Price = mealDto.Price,
                CategoryId = mealDto.CategoryId,
                OldPrice = mealDto.OldPrice,
        };
            _imgService.SetImage(meal, mealDto.MealImg);
            if (!string.IsNullOrEmpty(meal.Message))
                return new Meal { Message = meal.Message };
            
            await _context.AddAsync(meal);
            _context.SaveChanges();
            return meal;


        }

        public Meal DeleteMeal(Meal meal)
        {
            _imgService.DeleteImg(meal);
            _context.Remove(meal);
            _context.SaveChanges();
            return meal;
        }

        public async Task<Meal> GetMealByIdAsync(int id)
        {
            var meal = await _context.Meals.Include(meal => meal.Chef).Include(b=>b.Category).SingleOrDefaultAsync(meal => meal.Id == id);
            if (meal == null)
                return new Meal { Message=$"There is no Meal with Id :{id}"} ;
            return meal;
        }

        public MealByNameView GetMealByNameAsync(string name,string?token)
        {
            var meal = _context.Meals.SingleOrDefault(c=>c.Name==name);
            string isfavourite;
            if (string.IsNullOrEmpty(token))
            {
                isfavourite = null;
            }
            else
            {
                var userid = _wishlist.GetUserId(token);
                var wishlistid=_context.wishLists.SingleOrDefault(v=>v.UserId == userid);
                if( _context.WishListMeals.Any(w => w.MealId == meal.Id && w.WishListId == wishlistid.Id))
                {
                    isfavourite = "true";
                }
                else
                {
                    isfavourite= "false";
                }
            }
             
            
            var getmeal = _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).Include(m => m.MealReviews).Include(m => m.WishListMeals).Include(m => m.Additions).ThenInclude(c=>c.Choices).Include(c=>c.MealReviews).SingleOrDefault(c=>c.Name==name);
            var getbyname = new MealByNameView
            {
                Id = getmeal.Id,
                Description = getmeal.Description ?? null,
                ChefName = getmeal.Chef.Name,
                Image = getmeal.MealImgUrl,
                Name = getmeal.Name,
                Price = getmeal.Price,
                IsFavourite = isfavourite,
                CategoryName = getmeal.Category.Name,
                Rate = decimal.Round((getmeal.MealReviews.Sum(b => b.Rate) /
                getmeal.MealReviews.Where(b => b.Rate > 0).DefaultIfEmpty().Count()), 1),
                NumOfRates = getmeal.MealReviews.Count(c => c.Rate > 0),
                OldPrice = getmeal.OldPrice == 0.00m ? null : getmeal.OldPrice,
                StaticMealAdditions = _context.StaticAdditions.ToList(),
                MealAdditions = getmeal.Additions.Select(x=>new AdditionView { Id=x.Id,Choices=x.Choices,Name=x.Name}).ToList(),
                Reviews = _mapper.Map<List<MealReviewView>>(getmeal.MealReviews),



            };


           return getbyname;

        }

        public async Task<MealView> UpdateMealAsync(Meal selectedmeal,UpdateMealDto meal)
        {
            var errormessages = ValidateHelper<UpdateMealDto>.Validate(meal);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealView { Message = errormessages };
            }
            if (meal.CategoryId != null)
            {
                if (await _context.Categories.AnyAsync(b => b.Id == meal.CategoryId))
                    return new MealView { Message = $"There is no Category with Id : {meal.CategoryId}!" };

            }
           
            if (meal.ChefId != null)
            {
                if (!await _context.Chefs.AnyAsync(c => c.Id == meal.ChefId))
                    return new MealView { Message = $"There is no Chef With Id:{meal.ChefId}" };
                selectedmeal.ChefId =(int) meal.ChefId;
            }
              _imgService. UpdateImg(selectedmeal, meal.MealImg);
            if(!string.IsNullOrEmpty(selectedmeal.Message))
                return new MealView { Message =selectedmeal.Message };
            selectedmeal.Price = meal.Price?? selectedmeal.Price;
            selectedmeal.Name = meal.Name??selectedmeal.Name;
            selectedmeal.CategoryId=meal.CategoryId?? selectedmeal.CategoryId;
            selectedmeal.OldPrice = meal.OldPrice ?? selectedmeal.OldPrice;
            
            _context.Update(selectedmeal);
            _context.SaveChanges();

            return new MealView
            {
                Id =selectedmeal .Id,
                ChefId = selectedmeal.ChefId,
                ChefName = selectedmeal.Chef.Name,
                MealImgUrl = selectedmeal.MealImgUrl,
                Name = selectedmeal.Name,
                Price = selectedmeal.Price,
                Categoryid = selectedmeal.CategoryId,
                CategoryName= selectedmeal.Category.Name,
                OldPrice= selectedmeal.OldPrice,

            };

        }
        
        
        
    }
}
