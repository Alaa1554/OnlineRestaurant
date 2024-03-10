
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
        public async Task<IActionResult> GetChefsByCategoryIdAsync(int id,[FromQuery] PaginateDto paginate) 
        {
            var chefs = await _chefService.GetChefsByCategoryIdAsync(id,paginate);
            return Ok(chefs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChefByIdAsync(int id)
        {
            var chef = await _chefService.GetChefByIdAsync(id);
            if(!string.IsNullOrEmpty(chef.Message))
                return NotFound(chef.Message);
            chef.ChefImgUrl = Path.Combine("https://localhost:7166", "images", chef.ChefImgUrl);
            return Ok(chef);
        }
        [HttpPost]
       
        public async Task<IActionResult> CreateChefAsync([FromForm] Chef dto)
        {
            
           var Chef = await _chefService.CreateChef(dto);
            if (!string.IsNullOrEmpty(Chef.Message))
            {
                return BadRequest(Chef.Message);
            }
            var Message = "تم اضافه الشيف بنجاح";
            return Ok(new { Chef, Message });
            
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChefAsync(int id, [FromForm] UpdateChefDto chef)
        {
            var getchef = await _chefService.GetChefByIdAsync(id);

            if (!string.IsNullOrEmpty(getchef.Message))
            {
                return NotFound(getchef.Message);
            }

            var UpdatedData = await _chefService.UpdateChefAsync(getchef, chef);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            var Message = "تم تعديل الشيف  بنجاح";
            return Ok(new { UpdatedData, Message });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChefAsync(int id)
        {
            
            var chef = await _chefService.GetChefByIdAsync(id);

            if (!string.IsNullOrEmpty(chef.Message))
            {
                return NotFound(chef.Message);
            }
            var DeletedData =await _chefService.DeleteChefAsync(chef);
            if (!string.IsNullOrEmpty(DeletedData.Message))
            {
                return BadRequest(DeletedData.Message);
            }
            var Message = "تم حذف الشيف بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
