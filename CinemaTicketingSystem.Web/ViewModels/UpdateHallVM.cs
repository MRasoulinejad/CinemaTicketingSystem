using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateHallVM
    {
        public int HallId { get; set; }

        [Required(ErrorMessage = "Hall name is required.")]
        [StringLength(100, ErrorMessage = "Hall name cannot exceed 100 characters.")]
        public string HallName { get; set; }

        public List<UpdateSectionVM> Sections { get; set; } = new List<UpdateSectionVM>();
    }
}
