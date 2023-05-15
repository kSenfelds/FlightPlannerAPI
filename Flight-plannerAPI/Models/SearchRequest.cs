namespace Flight_plannerAPI.Models
{
    public class SearchRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureDate { get; set; }
    }
}