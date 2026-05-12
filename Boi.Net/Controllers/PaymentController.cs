
using Boi.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.V2.Core;
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



        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _config["Stripe:WebhookSecret"];

            var signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signatureHeader))
            {
                Console.WriteLine("\n[ERROR] Stripe-Signature header is missing! Request did not come from Stripe.\n");
                return BadRequest();
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    endpointSecret,
                    throwOnApiVersionMismatch: false
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent?.Metadata != null && paymentIntent.Metadata.TryGetValue("OrderId", out var orderIdStr))
                    {
                        if (int.TryParse(orderIdStr, out int orderId))
                        {
                            await _service.UpdateOrderPaymentStatusAsync(orderId);
                            Console.WriteLine($"\n[SUCCESS] Order {orderId} updated to Paid in DB!\n");
                        }
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"\n[STRIPE ERROR] {e.Message}\n");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- CRITICAL WEBHOOK ERROR ---\nMESSAGE: {ex.Message}\nSTACK TRACE: {ex.StackTrace}\n");
                return StatusCode(500);
            }
        }



    }
}
