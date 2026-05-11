namespace Boi.Net.DTOs.OrderDTOs
{


    public class CreateOrderDto
    {
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }


    public class OrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
