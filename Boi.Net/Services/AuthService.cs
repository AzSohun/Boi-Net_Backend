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


        // RegisterUser
        public async Task<bool> Register(string Name, string Email, string Password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == Email);

            if(user != null)
            {
                return false;
            }
            if(user == null && _context.Users.ToListAsync() == null)
            {
                user?.UserRole = Role.SuperAdmin;
            }

            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new RegistrationDto
            {
                Email = user?.Email!,
                Name = user?.Name!,
                Password = Password
            };

            await _context.AddAsync(newUser);

            return true;

        }

    }
}
