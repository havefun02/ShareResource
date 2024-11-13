namespace ShareResource.Models.ViewModels
{
    public class GalleryViewModel
    {
        public List<ImgResultViewModel>? Imgs = new List<ImgResultViewModel>();
        public PaginationViewModel? Pagination { get; set; }
    }
}
