namespace ShareResource.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public int currentPage { get; set; }    


    }
}
