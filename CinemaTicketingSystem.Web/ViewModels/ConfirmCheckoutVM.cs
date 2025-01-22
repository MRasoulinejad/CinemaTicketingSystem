namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ConfirmCheckoutVM
    {
        public int ShowTimeId { get; set; }
        public List<int> SelectedSeatIds { get; set; } // List of selected seat IDs
    }
}
