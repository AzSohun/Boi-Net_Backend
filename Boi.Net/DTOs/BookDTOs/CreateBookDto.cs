using System.ComponentModel.DataAnnotations;

namespace Boi.Net.DTOs.BookDTOs
{
    public class CreateBookDto
    {

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        public string? Publisher { get; set; }

        public string? ISBN { get; set; }

        public decimal Price { get; set; }

        public IFormFile? ImageFile { get; set; }

        public int? PageCount { get; set; }

        public string? Genre { get; set; } = "General";

        public bool IsAvailable { get; set; } = true;

        public DateOnly? PublishDate { get; set; }

    }
}


// This User Dto contains all the necessary data to create a Book.


