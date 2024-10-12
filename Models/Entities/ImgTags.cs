namespace ShareResource.Models.Entities
{
    public class ImgTags
    {
        public string ImgId { get; set; }=string.Empty;
        public string TagId { get; set; }=string.Empty;
        public virtual Tag? Tags { get; set;} 
        public virtual Img? Imgs { get; set; }
    }
}
