using Boi.Net.DTOs.UserDTOs;
using Boi.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult> UpdateMyProfile([FromForm] UpdateUserDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
            {
                return Unauthorized("You are not Authorized");
            }

            await _userService.MyProfileUpdateAsync(userId, dto);

            return Ok(new
            {
                Message= "Profile updated successfully" 
            });

        }


        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (userId == null)
            {
                return Unauthorized(new { Message = "You are not Authorized." });
            }

            var IsDelete = await _userService.SoftDeleteMyAccountAsync(userId);

            if (IsDelete)
            {
                return BadRequest("Failed to Delete Successfully.");
            }

            return Ok(new
            {
                Message = "Profile Deleted Successful."
            });
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("admin/manage-user/{userId}")]
        public async Task<ActionResult> ManageUser(string userId, [FromBody] AdminUpdateUserDto dto)
        {

            var result = await _userService.ManageUserByAdminAsync(userId, dto);

            if (!result)
            {
                return Unauthorized(new { Message = "You are not authorized." });
            }

            return Ok(new {Message = "User Updated Successfully."});

        }



    }
}
