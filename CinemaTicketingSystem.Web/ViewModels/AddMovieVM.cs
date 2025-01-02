using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddMovieVM
    {
        [Required]
        [StringLength(100, ErrorMessage = "The Title cannot exceed 100 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The Genre cannot exceed 50 characters.")]
        public string Genre { get; set; }
        [StringLength(1000, ErrorMessage = "The Description cannot exceed 1000 characters.")]
        public string Description { get; set; }
        [Range(1, 300, ErrorMessage = "The Duration must be between 1 and 500 minutes.")]
        [Required]
        public int Duration { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly ReleaseDate { get; set; }
        [Required]
        public IFormFile Poster { get; set; }
        [Required]
        public string TrailerUrl { get; set; }
    }
}
