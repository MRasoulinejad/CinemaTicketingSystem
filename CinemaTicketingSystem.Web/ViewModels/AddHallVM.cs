using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddHallVM
    {
        [Required(ErrorMessage = "Please enter the hall name.")]
        [StringLength(50, ErrorMessage = "Hall name cannot exceed 50 characters.")]
        [Display(Name = "Hall Name")]
        public string HallName { get; set; }

        [Required(ErrorMessage = "Please select a theatre.")]
        [Display(Name = "Theatre ID")]
        public int TheatreId { get; set; }
    }
}
