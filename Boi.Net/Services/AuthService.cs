using Boi.Net.Data;
using Boi.Net.DTOs.AuthDTOs;
using Boi.Net.Model;
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

        private readonly BoiNetDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(BoiNetDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        // RegisterUser Service
        public async Task<bool> Registration(RegistrationDto registration)
        {
            // User এর জায়গায় Users দেওয়া হয়েছে
            var isUserExist = await _context.Users.AnyAsync(user => user.Email == registration.Email);

            if (isUserExist)
            {
                return false;
            }

            var anyUserExists = await _context.Users.AnyAsync();
            bool isFirstUser = !anyUserExists;

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(registration.Password);

            var newUser = new User
            {
                Email = registration.Email,
                UserName = registration.Email, // Identity-এর জন্য UserName রিকোয়ার্ড
                Name = registration.Name,
                PasswordHash = PasswordHash,
                UserRole = isFirstUser ? Role.SuperAdmin : Role.User
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return true;
        }



        // Login Service
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

            if (user == null)
            {
                return null!;
            }

            bool isPsswordMatched = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPsswordMatched)
            {
                return null!;
            }

            var accessToken = CreateAccessToken(user);
            var refreshToken = CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            
            _context.Update(user);
            await _context.SaveChangesAsync();

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

            var user = await _context.Users.FirstOrDefaultAsync(user => user.RefreshToken == oldRefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null!;
            }

            string newAccessToken = CreateAccessToken(user);
            string newRefreshToken = CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _context.Update(user);
            await _context.SaveChangesAsync();


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
