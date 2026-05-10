using AutoMapper;
using Boi.Net.DTOs.AuthDTOs;
using Boi.Net.DTOs.UserDTOs;
using Boi.Net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _service;
        private readonly IMapper _mapper;

        public AuthController(AuthService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationDto registerUser)
        {

            var isSuccess = await _service.Registration(registerUser);

            if (!isSuccess)
            {
                return BadRequest("This User already exists");
            }

            return Ok(new {Message = "User Registration Successfull"});

        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginUser)
        {

            var user = await _service.Login(loginUser);

            if(user == null)
            {
                return BadRequest("Invalid Credential");
            }


            


            return Ok(user);

        }



        // Set Refresh Token into the Cookie
        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Javascript's XSS will unable read the token
                Expires = DateTime.UtcNow.AddDays(7), // Expire after 7 days
                Secure = true, // HTTPS Only
                SameSite = SameSiteMode.Strict // To protect from CPRF attack
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
