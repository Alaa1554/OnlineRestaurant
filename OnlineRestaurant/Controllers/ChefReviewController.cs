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
    public class ChefReviewController : ControllerBase
    {
        private readonly IChefReviewService _chefReviewService;
        private readonly ApplicationDbContext _context;

        public ChefReviewController(IChefReviewService chefReviewService, ApplicationDbContext context)
        {
            _chefReviewService = chefReviewService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAsync(int id,[FromQuery] PaginateDto paginate)
        {
            var reviews =  _chefReviewService.GetReviews(id,paginate);
            bool nextPage = false;
            if (reviews.Count() > paginate.Size)
            {
                reviews = reviews.Take(reviews.Count() - 1);
                nextPage = true;
            }
            var numOfChefReviews = await _context.ChefReviews.CountAsync(c=>c.ChefId==id);
            var numOfPages = (int)Math.Ceiling((decimal)numOfChefReviews / paginate.Size);
            return Ok(new { Reviews = reviews, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpPost]

        public async Task<IActionResult> CreateReviewAsync([FromHeader]string token, [FromBody] ChefReview dto)
        {
            var review = await _chefReviewService.CreateReview(token, dto);
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
            var review = await _chefReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(review.Message))
            {
                return NotFound(review.Message);
            }

            var result = _chefReviewService.UpdateReviewAsync(review, dto);
            if (!string.IsNullOrEmpty(result.Message))
            {
                return BadRequest(result.Message);
            }
            var message = "تم تعديل تعليقك بنجاح";
            return Ok(new { result, message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {

            var review = await _chefReviewService.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(review.Message))
            {
                return NotFound(review.Message);
            }
            var deletedData = _chefReviewService.DeleteReviewAsync(review);
            var message = "تم حذف تعليقك بنجاح";
            return Ok(new { deletedData, message });
        }
    }
}

