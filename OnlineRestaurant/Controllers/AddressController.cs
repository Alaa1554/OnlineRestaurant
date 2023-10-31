
using Microsoft.AspNetCore.Mvc;

using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private IAddressService _addressService;
        

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
          
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddressesAsync([FromHeader] string token)
        {
            var addresses = await _addressService.GetAddressesAsync(token);
            if(addresses==null)
            {
                return BadRequest("No User is Found");
            }
            
            return Ok(addresses);
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

            var UpdatedData =  _addressService.UpdateAddressAsync(getaddress, dto);
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
            var DeletedData = _addressService.DeleteAddress(address);
            var Message = "تم حذف العنوان بنجاح";
            return Ok(new { DeletedData, Message });
        }
    }
}
