namespace ShareResource.Models.Dtos
{
    public class VidDto
    {
        public IFormFile source { set; get; }
        public string FileName {  get; set; }
        public int start {  get; set; }
        public int end { get; set; }
    }
}
