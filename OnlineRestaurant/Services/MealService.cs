using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;


namespace OnlineRestaurant.Services
{
    public class MealService : IMealService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imgService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealService(ApplicationDbContext context, IImageService imgService, IMapper mapper, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _context = context;
            _imgService = imgService;
            _mapper = mapper;
            _userManager = userManager;
            _authService = authService;
        }
        public async Task<MealDto> CreateMeal(InsertMealDto mealDto)
        {
            if (!await _context.Chefs.AnyAsync(chef=>chef.Id==mealDto.ChefId))
                return new MealDto { Message = $"There is no Chef with Id : {mealDto.ChefId}!"};
            if (!await _context.Categories.AnyAsync(b=>b.Id==mealDto.CategoryId))
                return new MealDto { Message = $"There is no Category with Id : {mealDto.CategoryId}!" };
            if (await _context.Meals.AnyAsync(m => m.Name == mealDto.Name.Trim()))
                return new MealDto { Message = "يوجد وجبه اخري مسجله بهذا الاسم" };
            var meal = _mapper.Map<Meal>(mealDto);
            meal.MealImgUrl=_imgService.Upload(mealDto.MealImg);
            await _context.AddAsync(meal);
            await _context.SaveChangesAsync();
            return _mapper.Map<MealDto>(meal);
        }

        public async Task<MealDto> DeleteMeal(string name)
        {
            var meal=await GetMealByNameAsync(name);
            if (meal == null)
                return new MealDto { Message = "لم يتم العثور علي اي وجبه" };
            _imgService.Delete(meal.MealImgUrl);
            _context.Remove(meal);
            await _context.SaveChangesAsync();
            return _mapper.Map<MealDto>(meal);
        }

        public async Task<MealByNameView> GetMealByNameAsync(string name,string?token)
        {
            var meal = await _context.Meals.Include(m=>m.Chef).Include(m=>m.Category).Include(m=>m.MealReviews).Include(m=>m.Additions).ThenInclude(ma=>ma.Choices).SingleOrDefaultAsync(c=>c.Name==name.Trim());
            if (meal == null)
                return new MealByNameView { Message = "لا توجد نتائج بهذا الاسم" };
            bool isFavourite=false;
            if (!string.IsNullOrEmpty(token))
            {
                var userId = _authService.GetUserId(token);
                if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
                    return new MealByNameView { Message = "No User is Found!" };
                var wishlistid = await _context.wishLists.SingleOrDefaultAsync(v => v.UserId == userId);
                if (await _context.WishListMeals.AnyAsync(w => w.MealId == meal.Id && w.WishListId == wishlistid.Id))
                    isFavourite = true;
            }
            var staticAdditions = await _context.StaticAdditions.ToListAsync();
            return _mapper.Map< MealByNameView>(new MapMealDto { Meal=meal,StaticMealAdditions=staticAdditions,IsFavourite=isFavourite });
        }

        private async Task<Meal> GetMealByNameAsync(string name)
        {
           return await _context.Meals.Include(meal => meal.Chef).Include(b => b.Category).SingleOrDefaultAsync(meal => meal.Name == name.Trim());
        }

        public async Task<MealDto> UpdateMealAsync(string name,InsertMealDto dto)
        {
            var meal = await GetMealByNameAsync(name);
            if (meal == null)
                return new MealDto { Message = "لم يتم العثور علي اي وجبه" };
            if (!await _context.Categories.AnyAsync(b => b.Id == dto.CategoryId))
                return new MealDto { Message = $"There is no Category with Id : {dto.CategoryId}!" };
            if (!await _context.Chefs.AnyAsync(c => c.Id == dto.ChefId))
                return new MealDto { Message = $"There is no Chef With Id:{dto.ChefId}" };
            if (dto.Name!=meal.Name&&await _context.Meals.AnyAsync(m => m.Name == dto.Name.Trim()))
                return new MealDto { Message = "يوجد وجبه اخري مسجله بهذا الاسم" };
            _mapper.Map(dto, meal);
            meal.MealImgUrl = dto.MealImg == null ? meal.MealImgUrl : _imgService.Update(meal.MealImgUrl, dto.MealImg);
            await _context.SaveChangesAsync();
            return _mapper.Map<MealDto>(meal);
        }
    }
}
