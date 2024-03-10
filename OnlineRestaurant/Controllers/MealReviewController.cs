
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealReviewController : ControllerBase
    {
        private readonly IMealReviewService _mealReviewService;
        private readonly ApplicationDbContext _context;

        public MealReviewController(IMealReviewService mealReviewService, ApplicationDbContext context)
        {
            _mealReviewService = mealReviewService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAsync(int id,[FromQuery] PaginateDto paginate)
        {

            var Reviews = _mealReviewService.GetReviews(id, paginate);
            bool nextPage = false;
            if (Reviews.Count() > paginate.Size)
            {
                Reviews = Reviews.Take(Reviews.Count() - 1);
                nextPage = true;
            }
            var numOfMealReviews = await _context.MealReviews.CountAsync(c => c.MealId == id);
            var numOfPages = (int)Math.Ceiling((decimal)numOfMealReviews / paginate.Size);
            return Ok(new { Reviews = Reviews, NextPage = nextPage, NumOfPages = numOfPages });
        }
        [HttpPost]

        public async Task<IActionResult> CreateReviewAsync([FromHeader] string token,[FromBody] MealReview review)
        {

            var Review = await _mealReviewService.CreateReview(token,review);
            if (!string.IsNullOrEmpty(Review.Message))
            {
                return BadRequest(Review.Message);
            }
            var Message = "تم اضافه تعليقك بنجاح";
            return Ok(new { Review, Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewAsync(int id, [FromBody] UpdateReviewDto dto)
        {
            var GetReview = await _mealReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(GetReview.Message))
            {
                return NotFound(GetReview.Message);
            }

            var UpdatedData = await _mealReviewService.UpdateReviewAsync(GetReview, dto);
            
            var Message = "تم تعديل تعليقك بنجاح";
            return Ok(new { UpdatedData, Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {

            var GetReview = await _mealReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(GetReview.Message))
            {
                return NotFound(GetReview.Message);
            }
            var DeletedData = await _mealReviewService.DeleteReviewAsync(GetReview);
            var Message = "تم حذف تعليقك بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
