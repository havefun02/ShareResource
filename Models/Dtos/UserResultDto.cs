using ShareResource.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class UserResultDto
    {
        public string UserName { get; set; } = string.Empty; // Username of the user
        public string UserEmail { get; set; } = string.Empty; // Email of the user
        public string UserPhone { get; set; } = string.Empty; // Phone number of the user (optional)
    }
}
