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
            if (await _context.Meals.AnyAsync(m => m.Name == mealDto.Name.Trim()))
                return new Meal { Message = "يوجد وجبه اخري مسجله بهذا الاسم" };
            
            var meal = new Meal 
            { 
                Name =mealDto.Name,
                ChefId = mealDto.ChefId,
                Price = mealDto.Price,
                CategoryId = mealDto.CategoryId,
                OldPrice = mealDto.OldPrice,
                Description = mealDto.Description??null,
                Rate=0.00m,
                NumOfRate=0,
                
            };
            _imgService.SetImage(meal, mealDto.MealImg);
            if (!string.IsNullOrEmpty(meal.Message))
                return new Meal { Message = meal.Message };
            
            await _context.AddAsync(meal);
           await _context.SaveChangesAsync();
            return meal;


        }

        public async Task<Meal> DeleteMeal(Meal meal)
        {
            _imgService.DeleteImg(meal);
            _context.Remove(meal);
           await _context.SaveChangesAsync();
            return meal;
        }

        public async Task<Meal> GetMealByIdAsync(int id)
        {
            var meal = await _context.Meals.Include(meal => meal.Chef).Include(b=>b.Category).SingleOrDefaultAsync(meal => meal.Id == id);
            if (meal == null)
                return new Meal { Message=$"There is no Meal with Id :{id}"} ;
            return meal;
        }

        public async Task<MealByNameView> GetMealByNameAsync(string name,string?token)
        {
            var meal = await _context.Meals.SingleOrDefaultAsync(c=>c.Name==name.Trim());
            if (meal == null)
                return new MealByNameView { Message = "لا توجد نتائج بهذا الاسم" };
            bool isfavourite=false;
            if (string.IsNullOrEmpty(token))
            {
                isfavourite = false;
            }
            else
            {
                var userid = _authservice.GetUserId(token);
                if (!await _userManager.Users.AnyAsync(c => c.Id == userid))
                {
                    return new MealByNameView { Message = "No User is Found!" };
                }
                var wishlistid=await _context.wishLists.SingleOrDefaultAsync(v=>v.UserId == userid);
                if(await _context.WishListMeals.AnyAsync(w => w.MealId == meal.Id && w.WishListId == wishlistid.Id))
                {
                    isfavourite = true;
                }
                else
                {
                    isfavourite= false;
                }
            }
             
            var chef=await _context.Chefs.SingleOrDefaultAsync(c=>c.Id==meal.ChefId);
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == meal.CategoryId);
            var reviews=_context.MealReviews.Where(r=>r.MealId==meal.Id);
            var additions = _context.MealAdditions.Include(c=>c.Choices).Where(m=>m.MealId==meal.Id);
          
            var mealview = new MealByNameView
            {
                Id = meal.Id,
                Description = meal.Description ?? null,
                ChefName = chef.Name,
                Image = meal.MealImgUrl,
                Name = meal.Name,
                Price = meal.Price,
                IsFavourite = isfavourite,
                CategoryName = category.Name,
                Rate = meal.Rate,
                NumOfRates = meal.NumOfRate,
                OldPrice = meal.OldPrice,
                StaticMealAdditions = _context.StaticAdditions.ToList(),
                MealAdditions = additions.Select(x=>new AdditionView { Id=x.Id,Choices=x.Choices,Name=x.Name}).ToList(),
                Reviews = _mapper.Map<List<MealReviewView>>(reviews),



            };


           return mealview;

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
            await _context.SaveChangesAsync();

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
                Rate= selectedmeal.Rate,
                NumOfRate= selectedmeal.NumOfRate,

            };

        }
        
        
        
    }
}
