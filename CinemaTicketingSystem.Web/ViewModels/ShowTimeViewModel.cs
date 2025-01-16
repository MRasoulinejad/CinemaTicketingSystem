namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ShowTimeViewModel
    {
        public int ShowTimeId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public string HallName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Price { get; set; }
    }
}
