using System.ComponentModel.DataAnnotations;

namespace Boi.Net.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
        public string? Name { get; set; }

        public DateOnly? DOB { get; set; }

        // ফাইলের জন্য সঠিক নাম
        public IFormFile? ProfilePhoto { get; set; }
    }
}
