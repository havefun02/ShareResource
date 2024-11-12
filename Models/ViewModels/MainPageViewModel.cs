using CRUDFramework.Cores;

namespace ShareResource.Models.ViewModels
{
    public class MainPageViewModel
    {
        public List<ImgResultViewModel>? Imgs =new List<ImgResultViewModel>();
        public PaginationViewModel? Pagination { get; set; }
        public UserViewModel? User { set; get; }

    }
}
