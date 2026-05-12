
using Boi.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly PaymentService _service;
        private readonly IConfiguration _config;

        public PaymentController(PaymentService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }


        // Payment Starting Endpoint
        [Authorize]
        [HttpPost("create-payment-intent/{orderId}")]
        public async Task<ActionResult> CreatePaymentIntent(int orderId)
        {

            // Get UserId from the Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get Client Secret from the Service Layer
            var clientSecret = await _service.CreatePaymentIntentAsync(orderId, userId!);

            return Ok(new { clientSecret });

        }



        // Recieve Payment Confirmation Endpoint from Stripe
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _config["Stripe:WebhookSecret"];

            // ডিবাগিং লগ: আসল সমস্যা ধরার জন্য
            Console.WriteLine("\n--- WEBHOOK DEBUG START ---");
            Console.WriteLine($"Secret from Config: '{endpointSecret}'");
            Console.WriteLine($"JSON Body Length: {json.Length}");
            Console.WriteLine($"Stripe Signature Header: {Request.Headers["Stripe-Signature"].ToString().Substring(0, 15)}...");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret,
                    throwOnApiVersionMismatch: false
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var orderIdStr = paymentIntent?.Metadata["OrderId"];

                    if (int.TryParse(orderIdStr, out int orderId))
                    {
                        await _service.UpdateOrderPaymentStatusAsync(orderId);
                        Console.WriteLine($"SUCCESS: Order {orderId} updated to Paid!");
                    }
                }

                Console.WriteLine("--- WEBHOOK DEBUG END ---\n");
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"STRIPE ERROR: {e.Message}");
                Console.WriteLine("--- WEBHOOK DEBUG END ---\n");
                return BadRequest($"Webhook Error: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERAL ERROR: {ex.Message}");
                Console.WriteLine("--- WEBHOOK DEBUG END ---\n");
                return BadRequest($"General Error: {ex.Message}");
            }
        }



    }
}
