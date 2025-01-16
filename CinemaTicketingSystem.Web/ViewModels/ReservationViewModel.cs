using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ReservationViewModel
    {
        public int? MovieId { get; set; } // Selected Movie ID (optional)
        public int? TheatreId { get; set; } // Selected Theatre ID (optional)
        public DateOnly ShowDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); // Default to today's date
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }

    }
}
