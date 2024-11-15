namespace ShareResource.Models.Entities
{
    public class Tag
    {
        public string TagId { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public virtual ICollection<ImgTags>? ImgTags { get; set; }

    }
}
