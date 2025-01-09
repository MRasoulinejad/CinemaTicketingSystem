using CinemaTicketingSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateTheatreVM
    {
        [Required]
        public int TheatreId { get; set; }

        [Required(ErrorMessage = "Theatre name is required.")]
        [StringLength(100, ErrorMessage = "Theatre name cannot exceed 100 characters.")]
        public string TheatreName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        public string CurrentImage { get; set; }

        public IFormFile TheatreImage { get; set; }

        // List of Halls
        public List<UpdateHallVM> Halls { get; set; } = new List<UpdateHallVM>();

        // List of Sections
        public List<UpdateSectionVM> Seats { get; set; } = new List<UpdateSectionVM>();

    }
}
