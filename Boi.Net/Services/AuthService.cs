using Boi.Net.Data;

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
        public async Task Register(string Name, string Email, string Password)
        {



        }

    }
}
