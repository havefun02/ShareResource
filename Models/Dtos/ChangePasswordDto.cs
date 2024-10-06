namespace ShareResource.Models.Dtos
{
    public class ChangePasswordDto
    {
        public string UserEmail { get; set; } = string.Empty; // Current password of the user

        public string CurrentPassword { get; set; } = string.Empty; // Current password of the user
        public string NewPassword { get; set; } = string.Empty; // New password to be set
    }
}
