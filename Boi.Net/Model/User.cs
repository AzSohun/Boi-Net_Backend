using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Boi.Net.Model
{
    public class User: IdentityUser
    {

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public string ProfilePhotoUrl { get; set; } = string.Empty;

        public string ProfilePhotoId { get; set; } = string.Empty;

        public DateOnly? DOB { get; set; }

        public Role UserRole { get; set; } = Role.User;

        // EF Core doen't support array directly in the database.
        // Here ICollection is the solution of create Many-to-Many relational table.
        public ICollection<Book> Wishlist { get; set; } = new List<Book>();

        // TODO:
        // public ICollection<Order> Orders { get; set; } = new List<Order>();

        public bool IsBlocked { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public enum Role
    {
        SuperAdmin,
        Admin,
        User
    }


}
