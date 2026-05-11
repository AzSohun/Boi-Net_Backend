using System.ComponentModel.DataAnnotations.Schema;

namespace Boi.Net.Model
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public User ? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Pending"; // Pending -> Proccesing -> Shipped -> Delivered
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Failed

        public string? StripePaymentIntentId { get; set; } // The Tracking Id From Stripe

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
