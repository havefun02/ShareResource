namespace ShareResource.Models.ViewModels
{
    public class FirstLoadingViewModel
    {
        public ICollection<ImgResultViewModel>? Imgs { get; set; }
        public PaginationViewModel ?pagination { get; set; }
    }
}
