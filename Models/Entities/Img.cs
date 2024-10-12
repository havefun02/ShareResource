namespace ShareResource.Models.Entities
{
    public class Img
    {
        public string ImgId { get; set; }=string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }  
        public DateTime UploadDate { get; set; }  

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

        public virtual ICollection<ImgTags>? ImgTags { get; set; }


        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }  // If true, the image is private and access is restricted


    }
}
