using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddTheatreVM
    {
        [Required]
        [StringLength(100, ErrorMessage = "Theatre name cannot exceed 100 characters.")]
        public string TheatreName { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; }
        [Required]
        public IFormFile TheatreImage { get; set; }
    }
}
