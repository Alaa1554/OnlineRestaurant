using Microsoft.AspNetCore.Authorization;
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
    public class ChefController : ControllerBase
    {
        private readonly IChefService _chefService;
        private readonly ApplicationDbContext _context;

        public ChefController(IChefService chefService, ApplicationDbContext context)
        {
            _chefService = chefService;
            _context = context;
        }

        [HttpGet("GetAllChefs")]
        public async Task<IActionResult> GetAllChefsAsync([FromQuery]PaginateDto paginate)
        {
            var chefs = _chefService.GetChefs(paginate);
            bool nextPage = false;
            if (chefs.Count() > paginate.Size)
            {
                chefs = chefs.Take(chefs.Count() - 1);
                nextPage = true;
            }
            var numOfChefs = await _context.Chefs.CountAsync();
            var numOfPages = (int)Math.Ceiling((decimal)numOfChefs / paginate.Size);
            return Ok(new { Chefs = chefs, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpGet("GetChefsByCategoryId/{id}")]
        public IActionResult GetChefsByCategoryIdAsync(int id,[FromQuery] PaginateDto paginate) 
        {
            return Ok(_chefService.GetChefsByCategoryIdAsync(id, paginate));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChefByIdAsync(int id)
        {
            var chef = await _chefService.GetChefByIdAsync(id);
            if(chef==null)
                return NotFound($"No Chef is found with Id :{id}");
            return Ok(chef);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateChefAsync([FromForm] Chef dto)
        {
           var chef = await _chefService.CreateChef(dto);
            if (!string.IsNullOrEmpty(chef.Message))
                return BadRequest(chef.Message);

            var message = "تم اضافه الشيف بنجاح";
            return Ok(new { chef, message });
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChefAsync(int id, [FromForm] UpdateChefDto dto)
        {
            var result = await _chefService.UpdateChefAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result.Message);

            var message = "تم تعديل الشيف  بنجاح";
            return Ok(new { result, message });
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChefAsync(int id)
        {
            var deletedData =await _chefService.DeleteChefAsync(id);
            if (!string.IsNullOrEmpty(deletedData.Message))
                return BadRequest(deletedData.Message);

            var message = "تم حذف الشيف بنجاح";
            return Ok(new { deletedData, message });
        }
    }
}
