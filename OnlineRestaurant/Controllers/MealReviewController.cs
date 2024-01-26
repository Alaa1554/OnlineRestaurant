
using Microsoft.AspNetCore.Mvc;
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

        public MealReviewController(IMealReviewService mealReviewService)
        {
            _mealReviewService = mealReviewService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAsync(int id,[FromQuery] PaginateDto paginate)
        {

            var Reviews = await _mealReviewService.GetReviewsAsync(id, paginate);
            return Ok(Reviews);
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
