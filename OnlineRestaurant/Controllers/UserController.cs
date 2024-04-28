using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [HttpGet("GetAllUsers")]

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] PaginateDto paginate)
        {
            var users =await _userService.GetAllUsersAsync(paginate);
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
        [HttpGet("SearchForUserByName")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchForUserByName([FromQuery] SearchForUserByName searchForUser)
        {
            var users =await _userService.SearchForUserByName(searchForUser);
            if (!users.Any())
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
        [HttpPut("UpdateAccount")]
        [Authorize]
        public async Task<IActionResult> UpdateAccount([FromHeader] string token, [FromForm] UpdateAccountDto dto)
        {
            var user = await _userService.UpdateAccount(token, dto);
            if (!string.IsNullOrEmpty(user.Message))
            {
                return BadRequest(user.Message);
            }
            var message = "تم تحديث التغيرات بنجاح";
            return Ok(new { user, message });
        }
        [HttpPut("UpdatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePasswordAsync([FromHeader] string token, [FromBody] UpdatePasswordDto dto)
        {
            var errorMessage = await _userService.UpdatePassword(token, dto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            var message = "تم تغير كلمه السر بنجاح";
            return Ok(message);
        }
        [HttpDelete("DeleteAccount")]
        [Authorize]
        public async Task<IActionResult> DeleteAccountAsync([FromHeader] string token)
        {
            var errorMessage = await _userService.DeleteAccountAsync(token);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            var message = "تم حذف الحساب بنجاح";
            return Ok(message);
        }
        [HttpDelete("DeleteImage")]
        [Authorize]
        public async Task<IActionResult> DeleteImageAsync([FromHeader] string token)
        {
            var errorMessage = await _userService.DeleteUserImage(token);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok("تم حذف الصوره بنجاح");
        }
    }
}
