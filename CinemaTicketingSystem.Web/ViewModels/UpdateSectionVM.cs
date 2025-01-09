using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateSectionVM
    {

        public string SectionName { get; set; }

        public int SeatsCount { get; set; }
    }
}
