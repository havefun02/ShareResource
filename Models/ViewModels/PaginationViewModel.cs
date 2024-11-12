namespace ShareResource.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int totalItems { get; set; }
        public int currentPage { get; set; }    


    }
}
