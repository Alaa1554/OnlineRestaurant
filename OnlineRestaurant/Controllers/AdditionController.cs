
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
    public class AdditionController : ControllerBase
    {
        private readonly IMealAdditionService _additionService;
        private readonly ApplicationDbContext _context;

        public AdditionController(IMealAdditionService additionService, ApplicationDbContext context)
        {
            _additionService = additionService;
            _context = context;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAdditionAsync(int id, [FromQuery] PaginateDto paginate)
        {
            if (!_context.Meals.Any(m => m.Id == id))
            {
                return NotFound("لم يتم العثور علي اي وجبه");
            }
            var Additions = _additionService.GetMealAdditions(id, paginate);

            bool nextPage = false;
            if (Additions.Count() > paginate.Size)
            {
                Additions = Additions.Take(Additions.Count()-1);
                nextPage = true;
            }
            var numOfAdditions=await _context.MealAdditions.CountAsync(c=>c.Id==id);
            var numOfPages= (int)Math.Ceiling((decimal)numOfAdditions /paginate.Size);
            return Ok(new {Additions = Additions, NextPage = nextPage, NumOfPages = numOfPages });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAdditionAsync([FromBody] MealAddition Dto)
        {
            var Addition = await _additionService.CreateMealAddition(Dto);
            if (!string.IsNullOrEmpty(Addition.Message))
            {
                return BadRequest(Addition.Message);
            }
            var Message = "تم اضافه الاضافه بنجاح";
            return Ok(new { Addition, Message });

        }
        [HttpPut("{id}/{choiceid?}")]
        public async Task<IActionResult> UpdateAdditionAsync(int id, [FromBody] UpdateMealAdditionDto dto, int? choiceid)
        {
            var getaddition = await _additionService.GetMealAdditionByIdAsync(id);

            if (!string.IsNullOrEmpty(getaddition.Message))
            {
                return NotFound(getaddition.Message);
            }

            var UpdatedData = await _additionService.UpdateMealAdditionAsync(getaddition, dto, choiceid);
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
            var DeletedData = await _additionService.DeleteMealAddition(getaddition);
            var Message = "تم حذف الاضافه بنجاح";
            return Ok(new { DeletedData, Message });
        }
        [HttpDelete("DeleteChoice/{additionid}/{choiceid}")]
        public async Task<IActionResult> DeleteChoiceAsync(int additionid, int choiceid)
        {
            var Message = await _additionService.DeleteChoiceAsync(additionid, choiceid);
            if (!string.IsNullOrEmpty(Message))
            {
                return NotFound(Message);
            }
            return Ok("تم حذف الاختيار بنجاح");
        }
        [HttpPost("AddChoice/{AdditionId}")]
        public async Task<IActionResult> AddChoiceAsync(int AdditionId, [FromBody] Choice choice)
        {
            var Message=await _additionService.AddChoiceAsync(AdditionId, choice);
            if (!string.IsNullOrEmpty(Message))
            {
                return NotFound(Message);
            }
            return Ok("تم اضافه الاختيار بنجاح");
        }
        
    }
}
