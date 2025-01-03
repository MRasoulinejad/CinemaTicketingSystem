using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateMovieVM
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateOnly ReleaseDate { get; set; }
        [ValidateNever]
        public string Poster { get; set; }
        [ValidateNever]
        public IFormFile PosterFile { get; set; }
        public string TrailerUrl { get; set; }
    }
}
