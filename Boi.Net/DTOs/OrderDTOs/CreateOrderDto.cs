namespace Boi.Net.DTOs.OrderDTOs
{
    public class CreateOrderDto
    {
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
