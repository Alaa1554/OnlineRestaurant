using AutoMapper;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;


namespace OnlineRestaurant.Services
{
    public class MealService : IMealService
    {
        private readonly ApplicationDbContext _context;
        
        private readonly IImgService<Meal> _imgService;
        private readonly IAuthService _authservice;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealService(ApplicationDbContext context, IImgService<Meal> imgService, IMapper mapper, UserManager<ApplicationUser> userManager, IAuthService authservice)
        {
            _context = context;
            _imgService = imgService;
            
            _mapper = mapper;
            _userManager = userManager;
            _authservice = authservice;
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
                Description = mealDto.Description??null,
               
                
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
            if (meal == null)
                return new MealByNameView { Message = "لا توجد نتائج بهذا الاسم" };
            string isfavourite;
            if (string.IsNullOrEmpty(token))
            {
                isfavourite = null;
            }
            else
            {
                var userid = _authservice.GetUserId(token);
                if (!_userManager.Users.Any(c => c.Id == userid))
                {
                    return new MealByNameView { Message = "No User is Found!" };
                }
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
             
            
            var getmeal = _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).Include(m => m.MealReviews).Include(m => m.WishListMeal).Include(m => m.Additions).ThenInclude(c=>c.Choices).Include(c=>c.MealReviews).SingleOrDefault(c=>c.Name==name);
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
                if (!await _context.Categories.AnyAsync(b => b.Id == meal.CategoryId))
                    return new MealView { Message = $"There is no Category with Id : {meal.CategoryId}!" };
                selectedmeal.CategoryId = meal.CategoryId??selectedmeal.CategoryId;
            }
           
            if (meal.ChefId != null)
            {
                if (!await _context.Chefs.AnyAsync(c => c.Id == meal.ChefId))
                    return new MealView { Message = $"There is no Chef With Id:{meal.ChefId}" };
                selectedmeal.ChefId = meal.ChefId??selectedmeal.ChefId;
            }

              _imgService. UpdateImg(selectedmeal, meal.MealImg);
            if(!string.IsNullOrEmpty(selectedmeal.Message))
                return new MealView { Message =selectedmeal.Message };
            selectedmeal.Price = meal.Price?? selectedmeal.Price;
            selectedmeal.Name = meal.Name??selectedmeal.Name;
            
            selectedmeal.OldPrice = meal.OldPrice ?? selectedmeal.OldPrice;
            selectedmeal.Description = meal.Description ?? selectedmeal.Description;


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
                Description = selectedmeal.Description,

            };

        }
        
        
        
    }
}
