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
     public class ChefReviewService:IChefReviewService
     {
         private readonly ApplicationDbContext _context;
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public ChefReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)

        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ChefReviewView> CreateReview(ChefReview review)
         {
             var errormessages = ValidateHelper<ChefReview>.Validate(review);
             if (!string.IsNullOrEmpty(errormessages))
             {
                 return new ChefReviewView { Message = errormessages };
             }
            if (!await _context.Chefs.AnyAsync(chef => chef.Id == review.ChefId))
                return new ChefReviewView { Message = $"There is no Chef with Id : {review.ChefId}!" };

            var tokenHandler = new JwtSecurityTokenHandler();
             var jwtToken = tokenHandler.ReadJwtToken(review.TokenModel.Token) as JwtSecurityToken;

             var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
             var username = await _userManager.FindByIdAsync(userId);
            if (!await _context.ChefReviews.AnyAsync(c=>c.UserId == userId)&&await _context.ChefReviews.AnyAsync(c=>c.ChefId==review.ChefId))
            {
                return new ChefReviewView { Message = "You Already Have a Review" };
            }
             var Review = new ChefReview
             {
                 UserName = username.UserName,
                 CreatedDate = DateTime.UtcNow,
                 ChefId = review.ChefId,
                 Text = review.Text,
                 UserId = userId,
                 Rate = review.Rate,
             };
            var view = _mapper.Map<ChefReviewView>(Review);
             await _context.ChefReviews.AddAsync(Review);
             _context.SaveChanges();
            view.Id= Review.Id;
             return view;
         }

         public ChefReviewView DeleteReviewAsync(ChefReview review)
         {
             _context.ChefReviews.Remove(review);
             _context.SaveChanges();
            var view = _mapper.Map<ChefReviewView>(review); 
            
             return view;
         }

         public async Task<ChefReview> GetReviewByIdAsync(int id)
         {
             var Review = await _context.ChefReviews.FirstOrDefaultAsync(c => c.Id == id);
             if (Review == null)
                 return new ChefReview { Message = $"There is No Comment With Id:{id}" };
             return Review;

         }

         public async Task<IEnumerable<ChefReviewView>> GetReviewsAsync(int id)
         {
             var Reviews = await _context.ChefReviews.Where(c=>c.ChefId==id).ToListAsync();
            var views=_mapper.Map<IEnumerable<ChefReviewView>>(Reviews);
             return views;
         }

         public ChefReviewView UpdateReviewAsync(ChefReview review, UpdateReviewDto dto)
         {
             review.Text = dto.Text ?? review.Text;
            review.Rate = dto.Rate ?? review.Rate;
             _context.Update(review);
             _context.SaveChanges();
            var view=_mapper.Map<ChefReviewView>(review);
             return view;
         }
     }
 }
 

