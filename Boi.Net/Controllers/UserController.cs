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


        //[Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("all-profiles")]
        public async Task<ActionResult<UserDto[]>> GetAllProfiles()
        {
            try
            {
                var profiles = await _userService.GetAllProfile();

                if (profiles == null || profiles.Length == 0)
                {
                    return Ok(Array.Empty<UserDto>());
                }

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to fetch profiles.", Detail = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> GetMyProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized(new { Message = "You are not Authorized" });

                var result = await _userService.GetMyProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult> UpdateMyProfile([FromForm] UpdateUserDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized(new { Message = "You are not Authorized" });
                }

                var updatedUser = await _userService.MyProfileUpdateAsync(userId, dto);

                return Ok(new
                {
                    Message = "Profile updated successfully",
                    User = updatedUser
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized(new { Message = "You are not Authorized." });
                }

                var isDelete = await _userService.SoftDeleteMyAccountAsync(userId);

                if (!isDelete)
                {
                    return BadRequest(new { Message = "Failed to Delete Successfully." });
                }

                return Ok(new
                {
                    Message = "Profile Deleted Successful."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("admin/manage-user/{userId}")]
        public async Task<ActionResult> ManageUser(string userId, [FromBody] AdminUpdateUserDto dto)
        {
            try
            {
                var result = await _userService.ManageUserByAdminAsync(userId, dto);

                if (!result)
                {
                    return BadRequest(new { Message = "Failed to update user." });
                }

                return Ok(new { Message = "User Updated Successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        // All User Deletation (Testing Purpose)
        [AllowAnonymous]
        [HttpDelete("clear-all-users-completely")]
        public async Task<IActionResult> ClearAllUsersCompletely()
        {
            try
            {
                var isCleared = await _userService.HardClearUserTableAsync();

                if (!isCleared)
                {
                    return BadRequest(new { Message = "User table is already empty." });
                }

                return Ok(new { Message = "User table has been completely wiped out! Go ahead and register your fresh SuperAdmin." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to clear table.", Detail = ex.Message });
            }
        }
    }
}