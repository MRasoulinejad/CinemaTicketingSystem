using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class EditHallVM
    {
        public int HallId { get; set; }

        [Required(ErrorMessage = "Hall name is required.")]
        [StringLength(100, ErrorMessage = "Hall name cannot exceed 100 characters.")]
        public string HallName { get; set; }

        public List<EditSectionVM> Sections { get; set; } = new List<EditSectionVM>();
    }
}
