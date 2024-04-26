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

            var reviews = _mealReviewService.GetReviews(id, paginate);
            bool nextPage = false;
            if (reviews.Count() > paginate.Size)
            {
                reviews = reviews.Take(reviews.Count() - 1);
                nextPage = true;
            }
            var numOfMealReviews = await _context.MealReviews.CountAsync(c => c.MealId == id);
            var numOfPages = (int)Math.Ceiling((decimal)numOfMealReviews / paginate.Size);
            return Ok(new { Reviews = reviews, NextPage = nextPage, NumOfPages = numOfPages });
        }
        [HttpPost]

        public async Task<IActionResult> CreateReviewAsync([FromHeader] string token,[FromBody] MealReview dto)
        {
            var review = await _mealReviewService.CreateReview(token,dto);
            if (!string.IsNullOrEmpty(review.Message))
            {
                return BadRequest(review.Message);
            }
            var message = "تم اضافه تعليقك بنجاح";
            return Ok(new { review, message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewAsync(int id, [FromBody] UpdateReviewDto dto)
        {
            var result = await _mealReviewService.UpdateReviewAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
            {
                return NotFound(result.Message);
            }
            var message = "تم تعديل تعليقك بنجاح";
            return Ok(new { result, message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {
            var deletedData = await _mealReviewService.DeleteReviewAsync(id);
            if (!string.IsNullOrEmpty(deletedData.Message))
            {
                return NotFound(deletedData.Message);
            }
            var message = "تم حذف تعليقك بنجاح";
            return Ok(new { deletedData, message });
        }
    }
}
