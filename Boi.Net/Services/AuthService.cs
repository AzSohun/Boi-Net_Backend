using Boi.Net.Data;
using Boi.Net.DTOs.AuthDTOs;
using Boi.Net.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Boi.Net.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        // RegisterUser Service
        public async Task<bool> Registration(RegistrationDto registration)
        {
            var isUserExist = await _userManager.FindByEmailAsync(registration.Email);

            if (isUserExist != null)
            {
                return false;
            }

            var anyUserExists = await _userManager.Users.AnyAsync();
            bool isFirstUser = !anyUserExists;

            var newUser = new User
            {
                Email = registration.Email,
                UserName = registration.Email, 
                Name = registration.Name,
                UserRole = isFirstUser ? Role.SuperAdmin : Role.User
            };

            var result = await _userManager.CreateAsync(newUser, registration.Password);

            return result.Succeeded;
        }

        // Login Service
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return null!;
            }

            bool isPasswordMatched = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordMatched)
            {
                return null!;
            }

            var accessToken = CreateAccessToken(user);
            var refreshToken = CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            var loggedInUser = new AuthResponseDto
            {
                User = new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.UserRole.ToString()
                },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return loggedInUser;
        }

        // Create Token
        private string CreateAccessToken(User user)
        {
            var claim = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credential
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AuthResponseDto> GenerateNewTokens(string oldRefreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == oldRefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null!;
            }

            string newAccessToken = CreateAccessToken(user);
            string newRefreshToken = CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                User = new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.UserRole
                },
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}