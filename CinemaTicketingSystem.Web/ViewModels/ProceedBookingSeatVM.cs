namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ProceedBookingSeatVM
    {
        public int ShowTimeId { get; set; }
        public int SeatCount { get; set; }
        public string HallName { get; set; }
        public List<SeatVM> Seats { get; set; } = new List<SeatVM>();
        public List<SectionVM> Sections { get; set; }
    }
}
