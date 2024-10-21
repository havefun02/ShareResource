namespace ShareResource.Models.ViewModels
{
    public class ImgResultViewModel
    {
            public string ImgId { get; set; }=string.Empty;
            public string FileName { get; set; }= string.Empty;
            public string FileUrl { get; set; } = string.Empty;
            public DateTime UploadDate { get; set; }
            public string AuthorName { get; set; } =string.Empty;
            public string AuthorEmail { get; set; } =string.Empty ;
    }
}
