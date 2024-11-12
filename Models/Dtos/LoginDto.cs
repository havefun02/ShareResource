using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class LoginDto
    {
            [Required]
            [DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "Please input an email address")]
            public string? UserEmail { get; set; }
            public string? UserPassword { get; set; }
    }
}
