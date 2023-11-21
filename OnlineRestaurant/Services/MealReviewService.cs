using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Views;
using System.IdentityModel.Tokens.Jwt;


namespace OnlineRestaurant.Services
{
    public class MealReviewService : IMealReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public MealReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<MealReviewView> CreateReview(string token,MealReview review)
        {
            var errormessages = ValidateHelper<MealReview>.Validate(review);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new MealReviewView { Message = errormessages };
            }
            if (!await _context.Meals.AnyAsync(meal=>meal.Id==review.MealId))
                return new MealReviewView { Message = $"There is no Meal with Id : {review.MealId}!" };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;
            
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var user =await _userManager.FindByIdAsync(userId);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new MealReviewView { Message = "No User is Found!" };
            }
            if (await _context.MealReviews.AnyAsync(m=>m.UserId == userId)&&await _context.MealReviews.AnyAsync(m=>m.MealId==review.MealId))
            {
                return new MealReviewView { Message = "You Already Have a Review" };
            }
            var Review = new MealReview
            {
                UserName=user.UserName,
                CreatedDate = DateTime.Now,
                MealId=review.MealId,
                Text=review.Text,
                UserId=userId,
                Rate=review.Rate,
                UserImg=user.UserImgUrl

                
            };
            

              
            var view = _mapper.Map<MealReviewView>(Review);
            
            await _context.MealReviews.AddAsync(Review);
            var meal = await _context.Meals.Include(m => m.MealReviews).SingleOrDefaultAsync(m => m.Id == review.MealId);
            if (!meal.MealReviews.Any() || meal.Rate == 0.00m)

                meal.Rate = review.Rate;
            else
                meal.Rate = decimal.Round(meal.MealReviews.Sum(r => r.Rate) / meal.MealReviews.Where(r => r.Rate > 0).DefaultIfEmpty().Count(), 1);

            meal.NumOfRate++;
           await _context.SaveChangesAsync();
            view.Id = Review.Id;
            
            return view;
        }

        public async Task<MealReviewView> DeleteReviewAsync(MealReview review)
        {
            _context.MealReviews.Remove(review);
            var meal = await _context.Meals.Include(m => m.MealReviews).SingleOrDefaultAsync(m => m.Id == review.MealId);

            if (!meal.MealReviews.Any())

                meal.Rate = 0.00m;
            else
                meal.Rate = decimal.Round(meal.MealReviews.Sum(r => r.Rate) / meal.MealReviews.Where(r => r.Rate > 0).DefaultIfEmpty().Count(), 1);

            meal.NumOfRate--;
            await _context.SaveChangesAsync();
            var view = _mapper.Map<MealReviewView>(review);
            
            return view;
        }

        public async Task<MealReview> GetReviewByIdAsync(int id)
        {
           var Review= await _context.MealReviews.FirstOrDefaultAsync(c => c.Id == id);
            if (Review == null)
                return new MealReview { Message = $"There is No Comment With Id:{id}" };
            return Review;
            
        }

        public async Task<IEnumerable<MealReviewView>> GetReviewsAsync(int id)
        {
            var Reviews=await _context.MealReviews.Where(c=>c.MealId==id).ToListAsync();
            var views = _mapper.Map<IEnumerable< MealReviewView>>(Reviews);
            return views;
        }

        public async Task<MealReviewView> UpdateReviewAsync(MealReview review, UpdateReviewDto dto)
        {
            review.Text = dto.Text?? review.Text;
            if (dto.Rate.HasValue)
            {
                review.Rate =(decimal) dto.Rate;
                var meal = await _context.Meals.Include(m => m.MealReviews).SingleOrDefaultAsync(m => m.Id == review.MealId);
                if (meal.Rate == 0.00m)

                    meal.Rate =(decimal) dto.Rate;
                else
                    meal.Rate = decimal.Round(meal.MealReviews.Sum(r => r.Rate) / meal.MealReviews.Where(r => r.Rate > 0).DefaultIfEmpty().Count(), 1); 

            }
            _context.Update(review);
           await _context.SaveChangesAsync();

            var view=_mapper.Map<MealReviewView>(review);
            
            return view;
        }
        
        
    }
}
