namespace ShareResource.Models.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; } =string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public byte[] UserIcon { get; set; } = new byte[0];

    }
}
