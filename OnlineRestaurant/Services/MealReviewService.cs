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
    public class MealReviewService : IMealReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public MealReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<MealReviewView> CreateReview(string token,MealReview review)
        {
            if (!await _context.Meals.AnyAsync(meal=>meal.Id==review.MealId))
                return new MealReviewView { Message = $"There is no Meal with Id : {review.MealId}!" };
            var userId = _authService.GetUserId(token);
            var user =await _userManager.FindByIdAsync(userId);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
                return new MealReviewView { Message = "No User is Found!" };
            if (await _context.MealReviews.AnyAsync(m=>m.UserId == userId)&&await _context.MealReviews.AnyAsync(m=>m.MealId==review.MealId))
                return new MealReviewView { Message = "You Already Have a Review" };
            var mealReview = _mapper.Map<MealReview>(new MealReviewDto { MealReview = review,User=user,UserId=userId });
            await _context.MealReviews.AddAsync(mealReview);
            await _context.SaveChangesAsync();
            var view = _mapper.Map<MealReviewView>(mealReview);
            return view;
        }

        public async Task<MealReviewView> DeleteReviewAsync(int id)
        {
            var review = await GetReviewByIdAsync(id);
            if (review == null)
                return new MealReviewView { Message = $"There is No Comment With Id:{id}" };
            _context.MealReviews.Remove(review);
            await _context.SaveChangesAsync();
            var view = _mapper.Map<MealReviewView>(review);
            return view;
        }

        private async Task<MealReview> GetReviewByIdAsync(int id)
        {
           return await _context.MealReviews.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<MealReviewView> GetReviews(int id, PaginateDto dto)
        {
            var reviews= _context.MealReviews.Where(c=>c.MealId==id).Paginate(dto.Page, dto.Size).ToList();
            var views = _mapper.Map<IEnumerable< MealReviewView>>(reviews);
            return views;
        }

        public async Task<MealReviewView> UpdateReviewAsync(int id, UpdateReviewDto dto)
        {
            var review=await GetReviewByIdAsync(id);
             if (review == null)
                return new MealReviewView { Message = $"There is No Comment With Id:{id}" };
            _mapper.Map(dto,review);
            await _context.SaveChangesAsync();
            var view=_mapper.Map<MealReviewView>(review);
            return view;
        }
    }
}
