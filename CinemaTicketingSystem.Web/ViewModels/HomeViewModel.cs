using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class HomeViewModel
    {
        public List<Theatre> RandomTheatres { get; set; }
        public List<Movie> LatestMovies { get; set; }
    }
}
