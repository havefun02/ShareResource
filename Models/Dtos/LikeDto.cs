namespace ShareResource.Models.Dtos
{
    public class LikeDto
    {
        public string userId { get; set; } = string.Empty;
        public string resourceId { get; set; } = string.Empty;
        public bool state { set; get; }
    }
}
