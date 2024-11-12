using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class RegisterDto
    {
        public required string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter an email")]
        public required string UserEmail { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public required string UserPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
        public string UserPhone { get; set; } = string.Empty;

    }
}
