namespace ShareResource.Models.Entities
{
    public class ImgLovers
    {
        public string ImgId { get; set; } =string.Empty;
        public string UserId { set; get; }=string.Empty;    

        public virtual User? User { get; set; }
        public virtual Img? Img { set; get; }
    }
}
