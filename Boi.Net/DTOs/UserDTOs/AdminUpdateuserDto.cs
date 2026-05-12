using Boi.Net.Model;

namespace Boi.Net.DTOs.UserDTOs
{
    public class AdminUpdateUserDto
    {
        public Role UserRole { get; set; }
        public bool IsBlocked { get; set; }
    }
}
