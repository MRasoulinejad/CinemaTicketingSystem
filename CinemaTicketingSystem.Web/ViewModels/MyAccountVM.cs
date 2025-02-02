using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class MyAccountVM
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
