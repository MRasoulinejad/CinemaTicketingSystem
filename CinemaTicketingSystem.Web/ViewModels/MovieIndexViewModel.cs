using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Movie> AllMovies { get; set; }
        public List<Movie> LatestMovies { get; set; }
    }
}
