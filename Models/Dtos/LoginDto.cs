using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage ="Please input an email address")]
        public string Email { get; set; } = string.Empty; // Email of the user
        public string Password { get; set; } = string.Empty; // Password of the user
    }
}
