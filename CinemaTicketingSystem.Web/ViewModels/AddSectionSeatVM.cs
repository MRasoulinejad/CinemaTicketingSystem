using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddSectionSeatVM
    {
        [Required(ErrorMessage = "Hall ID is required.")]
        [Display(Name = "Hall ID")]
        public int HallId { get; set; }

        [Required(ErrorMessage = "Please enter the section name.")]
        [StringLength(50, ErrorMessage = "Section name cannot exceed 50 characters.")]
        [Display(Name = "Section Name")]
        public string SectionName { get; set; }

        [Required(ErrorMessage = "Please specify the number of seats.")]
        [Range(1, 200, ErrorMessage = "Number of seats must be between 1 and 200.")]
        [Display(Name = "Number of Seats")]
        public int NumberOfSeats { get; set; }
    }
}
