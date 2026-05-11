using Boi.Net.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;

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

            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(order => order.Book)
                .FirstOrDefaultAsync(order => order.Id == orderId && order.UserId == userId);


            if (order == null)
            {
                throw new Exception("Order is not is found or You don't have permission.");
            }


            // Stripe Always Counts Cents. So We Need Multiply by 100.
            long totalAmountInCents = (long)order.OrderItems.Sum(item => item.Book!.Price * item.Quantity) * 100;

            var options = new PaymentIntentCreateOptions
            {
                Amount = totalAmountInCents,
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", order.Id.ToString() }
                }
            };


            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options);

            return intent.ClientSecret;
        }


        public async Task UpdateOrderPaymentStatusAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if(order != null)
            {
                order.PaymentStatus = "Processing";
                order.OrderStatus = "Paid";
                await _context.SaveChangesAsync();

            }
        }

    }
}
