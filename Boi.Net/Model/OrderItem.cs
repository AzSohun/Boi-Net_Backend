using System.ComponentModel.DataAnnotations.Schema;

namespace Boi.Net.Model
{
    public class OrderItem
    {

        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

    }
}
