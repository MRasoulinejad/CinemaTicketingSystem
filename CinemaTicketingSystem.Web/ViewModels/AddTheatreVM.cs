using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddTheatreVM
    {
        [Required(ErrorMessage = "Please enter the theatre name.")]
        [StringLength(100, ErrorMessage = "Theatre name cannot exceed 100 characters.")]
        public string TheatreName { get; set; }

        [Required(ErrorMessage = "Please provide a location.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please upload an image for the theatre.")]
        public IFormFile TheatreImage { get; set; }
    }
}
