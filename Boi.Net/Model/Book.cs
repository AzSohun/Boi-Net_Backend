using System.ComponentModel.DataAnnotations;

namespace Boi.Net.Model
{
    public class Book
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string? Author { get; set; }

        public string? Publisher { get; set; }

        public string? ISBN { get; set; }
        
        public double Price { get; set; }

        public string? ImageUrl { get; set; }

        public int? PageCount { get; set; }

        public string? Genre { get; set; } = "General";

        public bool IsAvailable { get; set; } = true;

        public DateOnly? PublishDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
