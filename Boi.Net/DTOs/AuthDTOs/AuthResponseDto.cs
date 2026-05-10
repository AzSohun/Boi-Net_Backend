
namespace Boi.Net.DTOs.AuthDTOs
{
    public class AuthResponseDto
    {

        public object? User { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

    }
}
