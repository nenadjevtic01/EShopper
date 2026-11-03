namespace EShopper.Application.UseCases.DTO
{
    public class SearchReceiptDto : PagedSearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? UserId { get; set; }
    }
}
