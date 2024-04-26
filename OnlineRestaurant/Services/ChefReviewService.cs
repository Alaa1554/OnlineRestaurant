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
     public class ChefReviewService:IChefReviewService
     {
         private readonly ApplicationDbContext _context;
         private readonly UserManager<ApplicationUser> _userManager;
         private readonly IMapper _mapper;
         private readonly IAuthService _authService;
        public ChefReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthService authService)

        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ChefReviewView> CreateReview(string token,ChefReview review)
         {
            
            if (!await _context.Chefs.AnyAsync(chef => chef.Id == review.ChefId))
                return new ChefReviewView { Message = $"There is no Chef with Id : {review.ChefId}!" };

            var userId = _authService.GetUserId(token);
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new ChefReviewView { Message = "No User is Found!" };
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (await _context.ChefReviews.AnyAsync(m => m.UserId == userId) && await _context.ChefReviews.AnyAsync(m => m.ChefId == review.ChefId))
            {
                return new ChefReviewView { Message = "You Already Have a Review" };
            }
            var newReview = new ChefReview
             {
                 UserName = user.UserName,
                 CreatedDate = DateTime.UtcNow,
                 ChefId = review.ChefId,
                 Text = review.Text,
                 UserId= userId,
                 Rate = review.Rate,
                 UserImg= user.UserImgUrl
            };
             var view = _mapper.Map<ChefReviewView>(newReview);
             await _context.ChefReviews.AddAsync(newReview);
             _context.SaveChanges();
             view.Id= newReview.Id;
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
             var review = await _context.ChefReviews.FirstOrDefaultAsync(c => c.Id == id);
             if (review == null)
                 return new ChefReview { Message = $"There is No Comment With Id:{id}" };
             return review;

         }

         public IEnumerable<ChefReviewView> GetReviews(int id, PaginateDto dto)
         {
             var reviews = _context.ChefReviews.Where(c=>c.ChefId==id).Paginate(dto.Page,dto.Size).ToList();
             var views=_mapper.Map<IEnumerable<ChefReviewView>>(reviews);
             return views;
         }

         public ChefReviewView UpdateReviewAsync(ChefReview review, UpdateReviewDto dto)
         {
            _mapper.Map(dto, review);
            _context.SaveChanges();
             var view=_mapper.Map<ChefReviewView>(review);
             return view;
         }
     }
 }
 

