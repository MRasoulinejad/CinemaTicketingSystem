namespace CinemaTicketingSystem.Web.ViewModels
{
    public class FinalizeBookingRequestVM
    {
        public int ShowTimeId { get; set; }
        public List<int> SelectedSeatIds { get; set; }
    }
}
