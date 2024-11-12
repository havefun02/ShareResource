using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]

        public string? UserEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

    }
}
