using Boi.Net.DTOs.BookDTOs;

namespace Boi.Net.DTOs.AuthDTOs
{
    public class UserProfileResponseDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string ProfilePhotoUrl { get; set; } = string.Empty;

        public DateOnly? DOB { get; set; }

        public Role UserRole { get; set; } = Role.User;

        public ICollection<BookDto> Wishlist { get; set; } = new List<BookDto>();
    }
}
