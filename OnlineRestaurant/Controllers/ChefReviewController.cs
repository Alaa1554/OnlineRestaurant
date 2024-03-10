
using Microsoft.AspNet.Identity;
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

            var Reviews =  _chefReviewService.GetReviews(id,paginate);
            bool nextPage = false;
            if (Reviews.Count() > paginate.Size)
            {
                Reviews = Reviews.Take(Reviews.Count() - 1);
                nextPage = true;
            }
            var numOfChefReviews = await _context.ChefReviews.CountAsync(c=>c.ChefId==id);
            var numOfPages = (int)Math.Ceiling((decimal)numOfChefReviews / paginate.Size);
            return Ok(new { Reviews = Reviews, NextPage = nextPage,NumOfPages=numOfPages });
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
            var Message = "تم تعديل تعليقك بنجاح";
            return Ok(new { UpdatedData, Message });
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
            var Message = "تم حذف تعليقك بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}

