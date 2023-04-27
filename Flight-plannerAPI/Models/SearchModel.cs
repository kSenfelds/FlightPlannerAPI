namespace Flight_plannerAPI.Models
{
    public class SearchModel
    {
        public int Page { get; set; }
        public int TotalItems { get; set; }
        public Flight[] Items { get; set; }
    }
}
