using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class BookShowTimeVM
    {
        public ShowTime ShowTime {  get; set; }

        public Hall Hall { get; set; }

        public Movie Movie { get; set; }

        public Theatre Theatre { get; set; }

    }
}
