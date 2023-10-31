
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticAdditionController : ControllerBase
    {

        private IStaticMealAdditionService _additionService;
        

        public StaticAdditionController(IStaticMealAdditionService additionService)
        {
            _additionService = additionService;
           
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateAdditionAsync([FromForm] StaticMealAddition Dto)
        {
            var Addition = await _additionService.CreateMealAddition(Dto);
            if (!string.IsNullOrEmpty(Addition.Message))
            {
                return BadRequest(Addition.Message);
            }
            var Message = "تم اضافه الاضافه بنجاح";
            return Ok(new { Addition, Message });

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdditionAsync(int id, [FromForm] UpdateStaticMealAdditionDto dto)
        {
            var getaddition = await _additionService.GetMealAdditionByIdAsync(id);

            if (!string.IsNullOrEmpty(getaddition.Message))
            {
                return NotFound(getaddition.Message);
            }

            var UpdatedData = _additionService.UpdateMealAdditionAsync(getaddition, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            var Message = "تم تعديل الاضافه بنجاح";
            return Ok(new { UpdatedData, Message });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddition(int id)
        {
            var getaddition = await _additionService.GetMealAdditionByIdAsync(id);

            if (!string.IsNullOrEmpty(getaddition.Message))
            {
                return NotFound(getaddition.Message);
            }
            var DeletedData = _additionService.DeleteMealAddition(getaddition);
            var Message = "تم حذف الاضافه بنجاح";
            return Ok(new { DeletedData, Message });
        }

    }
}
