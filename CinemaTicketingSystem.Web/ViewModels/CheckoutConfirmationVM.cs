namespace CinemaTicketingSystem.Web.ViewModels
{
    public class CheckoutConfirmationVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public int ShowTimeId { get; set; }
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public string TheatreName { get; set; }
        public string Genre {  get; set; }
        public int Duration { get; set; }
        public string HallName { get; set; }
        public string ShowDate { get; set; }
        public string ShowTime { get; set; }
        public List<String> SelectedSeatNumbers { get; set; } // Add seat numbers for display
        public decimal TotalPrice { get; set; }
    }
}
