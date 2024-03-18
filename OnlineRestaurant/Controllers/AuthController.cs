
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
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var Message = await _authService.RegisterAsync(model);
            if (!string.IsNullOrEmpty(Message))
            {
                return BadRequest(Message);
            }
            
            return Ok("تم ارسال رمز التحقق الي بريدك الالكتروني");
        }
        [HttpPost("verifyAccount")]
        public async Task<IActionResult> VerifyAccountAsync([FromBody] VerifyAccountDto verifyAccount)
        {
            var User = await _authService.VerifyAccountAsync(verifyAccount);
            if(!string.IsNullOrEmpty(User.Message))
                return BadRequest(User.Message);
            return Ok(User);
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
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery]PaginateDto paginate)
        {
            var users = _authService.GetAllUsersAsync(paginate);
            bool nextPage = false;
            if (users.Count() > paginate.Size)
            {
                users = users.Take(users.Count() - 1);
                nextPage = true;
            }
            var numOfUsers = await _userManager.Users.CountAsync();
            var numOfPages = (int)Math.Ceiling((decimal)numOfUsers / paginate.Size);
            return Ok(new { Users = users, NextPage = nextPage, NumOfPages = numOfPages });
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
        [HttpGet("SearchForUserByName")]
        public async Task<IActionResult> SearchForUserByName([FromQuery]SearchForUserByName searchForUser)
        {
            var users = _authService.SearchForUserByName(searchForUser);
            if(!users.Any())
                return NotFound("لم يتم العثور علي اي مستخدم");
            bool nextPage = false;
            if (users.Count() > searchForUser.Size)
            {
                users = users.Take(users.Count() - 1);
                nextPage = true;
            }
            var numOfUsers = await _userManager.Users.CountAsync(c => c.UserName.Contains(searchForUser.UserName.ToLower().Trim()));
            var numOfPages = (int)Math.Ceiling((decimal)numOfUsers / searchForUser.Size);
            return Ok(new { Users = users, NextPage = nextPage, NumOfPages = numOfPages });
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
