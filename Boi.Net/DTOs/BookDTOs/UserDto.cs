using Boi.Net.Model;
using System.ComponentModel.DataAnnotations;

namespace Boi.Net.DTOs.BookDTOs
{
    public class UserDto
    {
        public Guid id { get; set; }

        public string Email { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;

        public string ProfilePhotoUrl { get; set; } = string.Empty;

        public string ProfilePhotoId { get; set; } = string.Empty;

        public DateOnly? DOB { get; set; }

        public Role UserRole { get; set; } = Role.User;

        public ICollection<BookDto> Wishlist { get; set; } = new List<BookDto>();

    }
}
