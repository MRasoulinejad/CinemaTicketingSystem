namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ConfirmCheckoutVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int ShowTimeId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTimeStart { get; set; }
        public TimeSpan ShowTimeEnd { get; set; }
        public List<string> SelectedSeats { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
