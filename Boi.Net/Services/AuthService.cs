using Boi.Net.Data;
using Boi.Net.DTOs.AuthDTOs;
using Boi.Net.Model;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Services
{
    public class AuthService
    {

        private readonly BoiNetDbContext _context;

        public AuthService(BoiNetDbContext context)
        {
            _context = context;
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
    

    }
}
