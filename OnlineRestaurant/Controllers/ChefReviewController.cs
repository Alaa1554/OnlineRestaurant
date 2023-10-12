using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChefReviewController : ControllerBase
    {
        private readonly IChefReviewService _chefReviewService;

        public ChefReviewController(IChefReviewService chefReviewService)
        {
            _chefReviewService = chefReviewService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAsync(int id)
        {

            var Reviews = await _chefReviewService.GetReviewsAsync(id);
            return Ok(Reviews);
        }
        [HttpPost]

        public async Task<IActionResult> CreateReviewAsync([FromHeader]string token, [FromBody] ChefReview review)
        {

            var Review = await _chefReviewService.CreateReview(token, review);
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
            var GetReview = await _chefReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(GetReview.Message))
            {
                return NotFound(GetReview.Message);
            }

            var UpdatedData = _chefReviewService.UpdateReviewAsync(GetReview, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            return Ok(UpdatedData);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {

            var GetReview = await _chefReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(GetReview.Message))
            {
                return NotFound(GetReview.Message);
            }
            var DeletedData = _chefReviewService.DeleteReviewAsync(GetReview);
            return Ok(DeletedData);
        }
    }
}

