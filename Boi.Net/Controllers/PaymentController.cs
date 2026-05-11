
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

            try
            {
                // Get UserId from the Token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get Client Secret from the Service Layer
                var clientSecret = await _service.CreatePaymentIntentAsync(orderId, userId!);

                return Ok(new { clientSecret });

            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }



        // Recieve Payment Confirmation Endpoint from Stripe
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = _config["Stripe:WebhookSecret"];

            try 
            {

                // Verify Signature
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                    );

                if(stripeEvent.Type == "payment_intent.succeeded")
                {

                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var orderIdStr = paymentIntent?.Metadata["OrderId"];


                    if(int.TryParse(orderIdStr, out int ordedId))
                    {
                        await _service.UpdateOrderPaymentStatusAsync(ordedId);
                    }

                }

                return Ok();

            }
            catch(StripeException e)
            {
                return BadRequest($"Webhook Error: {e.Message}");
            };
        }



    }
}
