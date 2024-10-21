namespace ShareResource.Models.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; } =string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public byte[] UserIcon { get; set; } = new byte[0];

    }
}
