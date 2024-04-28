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
            var additions = _additionService.GetMealAdditions(id, paginate);
            if(additions.Any(a=>a.Message!=null))
                return NotFound(additions.First().Message);
            bool nextPage = false;
            if (additions.Count() > paginate.Size)
            {
                additions = additions.Take(additions.Count()-1);
                nextPage = true;
            }
            var numOfAdditions=await _context.MealAdditions.CountAsync(c=>c.Id==id);
            var numOfPages= (int)Math.Ceiling((decimal)numOfAdditions /paginate.Size);
            return Ok(new {Additions = additions, NextPage = nextPage, NumOfPages = numOfPages });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdditionAsync([FromBody] MealAddition Dto)
        {
            var addition = await _additionService.CreateMealAddition(Dto);
            if (!string.IsNullOrEmpty(addition.Message))
                return BadRequest(addition.Message);
            var message = "تم اضافه الاضافه بنجاح";
            return Ok(new { addition, message });

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdditionAsync(int id, [FromBody] UpdateMealAdditionDto dto)
        {
            var result = await _additionService.UpdateMealAdditionAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result.Message);
            var message = "تم تعديل الاضافه بنجاح";
            return Ok(new { result, message });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAddition(int id)
        {
            var deletedData = await _additionService.DeleteMealAddition(id);
            if (!string.IsNullOrEmpty(deletedData.Message))
                return NotFound(deletedData.Message);

            var message = "تم حذف الاضافه بنجاح";
            return Ok(new { deletedData, message });
        }
        [HttpDelete("DeleteChoice/{choiceid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChoiceAsync(int choiceid)
        {
            var result = await _additionService.DeleteChoiceAsync(choiceid);
            if (!string.IsNullOrEmpty(result))
                return NotFound(result);
            return Ok("تم حذف الاختيار بنجاح");
        }
        [HttpPost("AddChoice")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddChoiceAsync([FromBody] Choice choice)
        {
            var result=await _additionService.AddChoiceAsync(choice);
            if (!string.IsNullOrEmpty(result))
                return NotFound(result);

            return Ok("تم اضافه الاختيار بنجاح");
        }
        
    }
}
