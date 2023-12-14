
using Microsoft.AspNetCore.Mvc;
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

        public ChefController(IChefService chefService)
        {
            _chefService = chefService;
        }

        
        [HttpGet("GetAllChefs")]
        public async Task<IActionResult> GetAllChefsAsync()
        {

            var chefs = await _chefService.GetChefsAsync();
             return Ok(chefs);
        }
        [HttpGet("GetChefsByCategoryId/{id}")]
        public async Task<IActionResult> GetChefsByCategoryIdAsync(int id) 
        {
            var chefs = await _chefService.GetChefsByCategoryIdAsync(id);
            return Ok(chefs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChefByIdAsync(int id)
        {
            var chef = await _chefService.GetChefByIdAsync(id);
            if(!string.IsNullOrEmpty(chef.Message))
                return NotFound(chef.Message);
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
