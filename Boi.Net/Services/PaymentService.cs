using Boi.Net.Data;

namespace Boi.Net.Services
{
    public class PaymentService
    {

        private readonly BoiNetDbContext _context;

        public PaymentService(BoiNetDbContext context)
        {
            _context = context;
        }


        public async Task<string> CreatePaymentIntentAsync(int orderId, string userId)
        {

            


            return "";
        }

    }
}
