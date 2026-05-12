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





    }
}
