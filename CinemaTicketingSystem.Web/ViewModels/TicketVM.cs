namespace CinemaTicketingSystem.Web.ViewModels
{
    public class TicketVM
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public string HallName { get; set; }
        public string SeatNumber { get; set; }
        public string ShowDate { get; set; }
        public string ShowTime { get; set; }
        public decimal Price { get; set; }
    }
}
