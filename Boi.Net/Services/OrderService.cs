using Boi.Net.Data;

namespace Boi.Net.Services
{
    public class OrderService
    {

        private readonly BoiNetDbContext _context;

        public OrderService(BoiNetDbContext context)
        {
            _context = context;
        }

    }
}
