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
    public class StaticAdditionController : ControllerBase
    {

        private readonly IStaticMealAdditionService _additionService;
        private readonly ApplicationDbContext _context;


        public StaticAdditionController(IStaticMealAdditionService additionService, ApplicationDbContext context)
        {
            _additionService = additionService;
            _context = context;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateAdditionAsync([FromForm] StaticMealAddition Dto)
        {
            var addition = await _additionService.CreateMealAddition(Dto);
            var message = "تم اضافه الاضافه بنجاح";
            return Ok(new { addition, message });

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateAdditionAsync(int id, [FromForm] UpdateStaticMealAdditionDto dto)
        {
            var result = await _additionService.UpdateMealAdditionAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
            {
                return NotFound(result.Message);
            }
            var message = "تم تعديل الاضافه بنجاح";
            return Ok(new { result, message });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteAddition(int id)
        {
            var deletedData = await _additionService.DeleteMealAddition(id);
            if (!string.IsNullOrEmpty(deletedData.Message))
            {
                return NotFound(deletedData.Message);
            }
            var message = "تم حذف الاضافه بنجاح";
            return Ok(new { deletedData, message });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginateDto paginate)
        {
            var additions =_additionService.GetAllAdditions(paginate);
            bool nextPage = false;
            if (additions.Count() > paginate.Size)
            {
                additions = additions.Take(additions.Count()-1);
                nextPage = true;
            }
            var numOfStaticAdditions = await _context.StaticAdditions.CountAsync();
            var numOfPages = (int)Math.Ceiling((decimal)numOfStaticAdditions / paginate.Size);
            return Ok(new { Additions = additions, NextPage = nextPage,NumOfPages=numOfPages });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var addition = await _additionService.GetMealAdditionByIdAsync(id);
            if (!string.IsNullOrEmpty(addition.Message))
            {
                return NotFound(addition.Message);
            }
            return Ok(addition);
        }

    }
}
