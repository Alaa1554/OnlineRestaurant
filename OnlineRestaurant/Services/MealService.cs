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
        
        private readonly IImageService _imgService;
        private readonly IAuthService _authservice;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealService(ApplicationDbContext context, IImageService imgService, IMapper mapper, UserManager<ApplicationUser> userManager, IAuthService authservice)
        {
            _context = context;
            _imgService = imgService;
            _mapper = mapper;
            _userManager = userManager;
            _authservice = authservice;
        }
        public async Task<MealDto> CreateMeal(Meal mealDto)
        {
            var errormessages=ValidateHelper<Meal>.Validate(mealDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealDto { Message = errormessages };
            }
            if (!await _context.Chefs.AnyAsync(chef=>chef.Id==mealDto.ChefId))
                return new MealDto { Message = $"There is no Chef with Id : {mealDto.ChefId}!"};
            if (!await _context.Categories.AnyAsync(b=>b.Id==mealDto.CategoryId))
                return new MealDto { Message = $"There is no Category with Id : {mealDto.CategoryId}!" };
            if (await _context.Meals.AnyAsync(m => m.Name == mealDto.Name.Trim()))
                return new MealDto { Message = "يوجد وجبه اخري مسجله بهذا الاسم" };
            
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
            meal.MealImgUrl=_imgService.Upload(mealDto.MealImg);
            if (!string.IsNullOrEmpty(meal.Message))
                return new MealDto { Message = meal.Message };
            
            await _context.AddAsync(meal);
            await _context.SaveChangesAsync();
            var mealView=_mapper.Map<MealDto>(meal);
            return mealView;


        }

        public async Task<MealDto> DeleteMeal(Meal meal)
        {
            _imgService.Delete(meal.MealImgUrl);
            _context.Remove(meal);
           await _context.SaveChangesAsync();
            var mealView = _mapper.Map<MealDto>(meal);
            return mealView;
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
            var staticAdditions = await _context.StaticAdditions.ToListAsync();
            var mealview = new MealByNameView
            {
                Id = meal.Id,
                Description = meal.Description ?? null,
                ChefName = chef.Name,
                Image = Path.Combine("https://localhost:7166", "images", meal.MealImgUrl),
                Name = meal.Name,
                Price = meal.Price,
                IsFavourite = isfavourite,
                CategoryName = category.Name,
                Rate = meal.Rate,
                NumOfRates = meal.NumOfRate,
                OldPrice = meal.OldPrice,
                StaticMealAdditions = _mapper.Map<List<StaticMealAdditionView>>(staticAdditions),
                MealAdditions = additions.Select(x=>new AdditionView { Id=x.Id,Choices=x.Choices,Name=x.Name}).ToList(),
                Reviews = _mapper.Map<List<MealReviewView>>(reviews),
                CategoryId= meal.CategoryId,
                ChefId= meal.ChefId,
            };


           return mealview;

        }

        public async Task<Meal> GetMealByNameAsync(string name)
        {
            var meal = await _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).SingleOrDefaultAsync(meal => meal.Name == name.Trim());
            if (meal == null)
                return new Meal { Message = $"There is no Meal with Name :{name}" };
            return meal;
        }

        public async Task<MealView> UpdateMealAsync(Meal selectedmeal,UpdateMealDto meal)
        {
            var mealview = new MealView();
            var errormessages = ValidateHelper<UpdateMealDto>.Validate(meal);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealView { Message = errormessages };
            }
           
            if (meal.CategoryId != null)
            {
                if (!await _context.Categories.AnyAsync(b => b.Id == meal.CategoryId))
                    return new MealView { Message = $"There is no Category with Id : {meal.CategoryId}!" };
                selectedmeal.CategoryId = (int)meal.CategoryId;
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == meal.CategoryId);
                mealview.CategoryName = category.Name;
            }
            else
                mealview.CategoryName=selectedmeal.Category.Name;
            
           
            if (meal.ChefId != null)
            {
                if (!await _context.Chefs.AnyAsync(c => c.Id == meal.ChefId))
                    return new MealView { Message = $"There is no Chef With Id:{meal.ChefId}" };
                selectedmeal.ChefId =(int) meal.ChefId;
                var chef = await _context.Chefs.FirstOrDefaultAsync(c=>c.Id== meal.ChefId);
                mealview.ChefName = chef.Name;
            }
            else
                mealview.ChefName=selectedmeal.Chef.Name;
            if (meal.Description != null && meal.Description.ToLower().Trim() == "undefined")
                selectedmeal.Description = null;
            else
                selectedmeal.Description = meal.Description ?? selectedmeal.Description;

            selectedmeal.MealImgUrl = meal.MealImg == null ? selectedmeal.MealImgUrl : _imgService.Update(selectedmeal.MealImgUrl, meal.MealImg);
            if(!string.IsNullOrEmpty(selectedmeal.Message))
                return new MealView { Message =selectedmeal.Message };
            selectedmeal.Price = meal.Price?? selectedmeal.Price;
            selectedmeal.Name = meal.Name??selectedmeal.Name;
            
            selectedmeal.OldPrice = meal.OldPrice ?? selectedmeal.OldPrice;
            
            _context.Update(selectedmeal);
            await _context.SaveChangesAsync();

            mealview.Id = selectedmeal.Id;
            mealview.ChefId =selectedmeal.ChefId;
            mealview.MealImgUrl = Path.Combine("https://localhost:7166", "images", selectedmeal.MealImgUrl);
            mealview.Name = selectedmeal.Name;
            mealview.Price = selectedmeal.Price;
            mealview.Categoryid = selectedmeal.CategoryId;
            mealview.OldPrice = selectedmeal.OldPrice;
            mealview.Description = selectedmeal.Description;
            mealview.Rate = selectedmeal.Rate;
            mealview.NumOfRate = selectedmeal.NumOfRate;

            return mealview;

        }
        
        
        
    }
}
