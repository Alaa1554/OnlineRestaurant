using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAllAsync(int id)
        {

            var Reviews = await _mealReviewService.GetReviewsAsync(id);
            return Ok(Reviews);
        }
        [HttpPost]

        public async Task<IActionResult> CreateReviewAsync([FromBody] MealReview review)
        {

            var Review = await _mealReviewService.CreateReview(review);
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

            var UpdatedData = _mealReviewService.UpdateReviewAsync(GetReview, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            return Ok(UpdatedData);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {

            var GetReview = await _mealReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(GetReview.Message))
            {
                return NotFound(GetReview.Message);
            }
            var DeletedData = _mealReviewService.DeleteReviewAsync(GetReview);
            return Ok(DeletedData);
        }
    }
}
