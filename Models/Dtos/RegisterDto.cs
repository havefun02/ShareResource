﻿using System.ComponentModel.DataAnnotations;

namespace ShareResource.Models.Dtos
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty; // Username of the user
        [EmailAddress(ErrorMessage = "Please enter an email")]
        public string Email { get; set; } = string.Empty; // Email of the user
        public string Password { get; set; } = string.Empty; // Password of the user
        [Phone(ErrorMessage ="Please enter a valid phone number")]
        public string UserPhone { get; set; } = string.Empty; // Phone number of the user (optional)

    }
}
