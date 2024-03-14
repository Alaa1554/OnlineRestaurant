
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
        public async Task<IActionResult> GetAllAddressesAsync([FromHeader] string token, [FromQuery] PaginateDto paginate)
        {
            var addresses = await _addressService.GetAddressesAsync(token,paginate);
            if(addresses==null)
            {
                return BadRequest("No User is Found");
            }
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
        public async Task<IActionResult> AddAddressAsync([FromHeader] string token,[FromBody] Address dto)
        {
            var Address = await _addressService.CreateAddressAsync(token, dto);
            if (!string.IsNullOrEmpty(Address.Message))
            {
                return BadRequest(Address.Message);
            }
            var Message = "تم اضافه العنوان بنجاح";
            return Ok(new { Address, Message });


        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddressAsync(int id, [FromBody] UpdateAddressDto dto)
        {
            var getaddress = await _addressService.GetAddressByIdAsync(id);

            if (!string.IsNullOrEmpty(getaddress.Message))
            {
                return NotFound(getaddress.Message);
            }

            var UpdatedData = await _addressService.UpdateAddressAsync(getaddress, dto);
            if (!string.IsNullOrEmpty(UpdatedData.Message))
            {
                return BadRequest(UpdatedData.Message);
            }
            var Message = "تم تعديل العنوان بنجاح";
            return Ok(new { UpdatedData, Message });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (!string.IsNullOrEmpty(address.Message))
            {
                return NotFound(address.Message);
            }
            var DeletedData =await _addressService.DeleteAddress(address);
            var Message = "تم حذف العنوان بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
