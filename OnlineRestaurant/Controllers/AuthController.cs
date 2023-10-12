using Microsoft.AspNetCore.Http;
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
            var user = await _authService.RegisterAsync(model);
            if (!user.IsAuthenticated)
            {
                return BadRequest(user.Message);
            }

            return Ok(user);
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
        [HttpPut("UpdateImg")]
        public async Task<IActionResult> UpdateImg([FromHeader] string token, [FromForm] IFormFile userimg)
        {
            var User = await _authService.UpdateImg(token,userimg);
            if (!string.IsNullOrEmpty(User.Message))
            {
                return BadRequest(User.Message);
            }
            var Message = "تم تحديث الصوره بنجاح";
            return Ok(new {User, Message});
        }
    }
}
