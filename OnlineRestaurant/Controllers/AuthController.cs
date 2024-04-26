
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using OnlineRestaurant.Services;

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
            var result = await _authService.RegisterAsync(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            
            return Ok("تم ارسال رمز التحقق الي بريدك الالكتروني");
        }
        [HttpPost("verifyAccount")]
        public async Task<IActionResult> VerifyAccountAsync([FromBody] VerifyAccountDto verifyAccount)
        {
            var user = await _authService.VerifyAccountAsync(verifyAccount);
            if(!string.IsNullOrEmpty(user.Message))
                return BadRequest(user.Message);
            return Ok(user);
        }
    
        [HttpPost("ResendVerificationCode")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] EmailDto emailDto)
        {
           var result = await _authService.ResendVerificationCode(emailDto.Email);
           if (!string.IsNullOrEmpty(result))
               return BadRequest(result);
           return Ok("تم ارسال رمز التحقق مره اخري");
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
      
        [HttpPost("gmail")]
        public async Task<IActionResult> GmailLogIn([FromForm] GmailRegisterDto dto)
        {
            var user=await _authService.GmailRegisterAsync(dto);
            if(!string.IsNullOrEmpty(user.Message))
                return BadRequest(user.Message);
            return Ok(user);
        }
       
        [HttpDelete("RemoveRole/{userid}")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> RemoveRoleAsync(string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RemoveRoleAsync(userid);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }
        
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody]EmailDto email)
        {
            var result= await _authService.ForgetPassword(email);
            if(!string.IsNullOrEmpty(result))
                return NotFound(result);
            return Ok("تم ارسال رابط التحقق الي بريدك الالكتروني");
            
        }
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto resetPassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPassword(resetPassword);
                if(!string.IsNullOrEmpty(result))
                    return BadRequest(result);
                return Ok("تم تغيير كلمه السر بنجاح");
            }
            return BadRequest();
        }
    }
}
