using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Movie> Movies { get; set; } // Currently loaded movies (for the current batch)
        public int Skip { get; set; }           // How many movies have been skipped so far
        public bool HasMore { get; set; }       // True if there are more movies to load
    }

}
