using Boi.Net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public async Task<ActionResult> CreatePaymentIntent(int orderId)
        {

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var clientSecret = await _service.CreatePaymentIntentAsync(orderId, userId!);

                return Ok(new { clientSecret });

            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

    }
}
