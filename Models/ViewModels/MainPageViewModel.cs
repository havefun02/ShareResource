using CRUDFramework.Cores;

namespace ShareResource.Models.ViewModels
{
    public class MainPageViewModel
    {
        public ICollection<ImgResultViewModel>? Imgs { get; set; }
        public PaginationViewModel? Pagination { get; set; }
        public UserViewModel? User { set; get; }

    }
}
