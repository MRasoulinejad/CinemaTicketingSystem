using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateTheatreSecondStepVM
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        [ValidateNever]
        public IFormFile TheatreImage { get; set; }
        [ValidateNever]
        public string CurrentImage { get; set; }
    }
}
