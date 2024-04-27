
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
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;


        public AddressController(IAddressService addressService, ApplicationDbContext context, IAuthService authService)
        {
            _addressService = addressService;
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAddressesAsync([FromHeader] string token, [FromQuery] PaginateDto paginate)
        {
            var addresses = await _addressService.GetAddressesAsync(token,paginate);
            if(addresses==null)
                return BadRequest("No User is Found");
            
            bool nextPage = false;
            if (addresses.Count() > paginate.Size)
            {
                addresses = addresses.Take(addresses.Count()-1);
                nextPage = true;
            }
            var userId=_authService.GetUserId(token);
            var numOfAddresses = await _context.Addresses.CountAsync(c => c.UserId == userId);
            var numOfPages = (int)Math.Ceiling((decimal)numOfAddresses / paginate.Size);
            return Ok(new {Addresses = addresses, NextPage = nextPage, NumOfPages = numOfPages });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAddressAsync([FromHeader] string token,[FromBody] Address dto)
        {
            var address = await _addressService.CreateAddressAsync(token, dto);
            if (!string.IsNullOrEmpty(address.Message))
                return BadRequest(address.Message);
            
            var message = "تم اضافه العنوان بنجاح";
            return Ok(new { address, message });
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddressAsync(int id, [FromBody] UpdateAddressDto dto)
        {
            var result = await _addressService.UpdateAddressAsync(id, dto);
            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result.Message);
            
            var message = "تم تعديل العنوان بنجاح";
            return Ok(new { result, message });
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result =await _addressService.DeleteAddress(id);
            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result.Message);
            
            var message = "تم حذف العنوان بنجاح";
            return Ok(new { result, message });
        }
    }
}
