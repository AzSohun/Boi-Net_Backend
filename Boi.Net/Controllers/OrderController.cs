using Boi.Net.DTOs.OrderDTOs;
using Boi.Net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderService _service;

        public OrderController(OrderService service)
        {
            _service = service;
        }

        [HttpPost("place-order")]
        public async Task<ActionResult> PlaceeOrder([FromBody] CreateOrderDto createOrderDto)
        {

            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _service.CreateOrderAsync(userId!, createOrderDto);

            return Ok(new {
            Message= "Order Placed Successfully",
            Orderid = order.Id,
            TotalAmount = order.TotalAmount
            });

        }
    }
}
