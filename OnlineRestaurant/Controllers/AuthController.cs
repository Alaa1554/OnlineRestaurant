
using Microsoft.AspNetCore.Mvc;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var User = await _authService.RegisterAsync(model);
            if (!User.IsAuthenticated)
            {
                return BadRequest(User.Message);
            }
            var Message = "تم التسجيل بنجاح";
            return Ok(new { User, Message });
        }
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync ([FromBody] TokenRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.GetTokenAsync(model);
            if (!user.IsAuthenticated)
            {
                return BadRequest(user.Message);
            }
            return Ok(user);
        }
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _authService.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(user))
            {
                return BadRequest(user);
            }

            return Ok(model);
        }
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromHeader] string token, [FromForm] UpdateAccountDto dto)
        {
            var User = await _authService.UpdateAccount(token,dto);
            if (!string.IsNullOrEmpty(User.Message))
            {
                return BadRequest(User.Message);
            }
            var Message = "تم تحديث التغيرات بنجاح";
            return Ok(new {User, Message});
        }
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccountAsync([FromHeader]string token)
        {
            var errormessages=await _authService.DeleteAccountAsync(token);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return BadRequest(errormessages);
            }
            var Message = "تم حذف الحساب بنجاح";
            return Ok(Message);
        }
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync([FromHeader]string token, [FromBody] UpdatePasswordDto dto)
        {
            var errormessage = await _authService.UpdatePassword(token, dto);
            if (!string.IsNullOrEmpty(errormessage))
            {
                return BadRequest(errormessage);
            }
            var Message = "تم تغير كلمه السر بنجاح";
            return Ok(Message);
        }
        [HttpDelete("DeleteImage")]
        public async Task<IActionResult> DeleteImageAsync([FromHeader]string token)
        {
            var message = await _authService.DeleteUserImage(token);
            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(message);
            }
            return Ok("تم حذف الصوره بنجاح");
        }
        [HttpPost("Gmail")]
        public async Task<IActionResult> GmailLogIn([FromForm] GmailRegisterDto dto)
        {
            var AuthModel=await _authService.GmailRegisterAsync(dto);
            if(!string.IsNullOrEmpty(AuthModel.Message))
                return BadRequest(AuthModel.Message);
            return Ok(AuthModel);
        }
    }
}
