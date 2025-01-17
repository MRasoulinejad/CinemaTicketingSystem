namespace CinemaTicketingSystem.Web.ViewModels
{
    public class SeatVM
    {
        public int SeatId { get; set; }
        public string SectionName { get; set; }
        public int SeatNumber { get; set; }
        public bool IsReserved { get; set; }
    }
}
