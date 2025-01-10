using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class EditSectionVM
    {
        public string OldSectionName { get; set; } // Holds the old section name

        public string SectionName { get; set; }

        public int SeatsCount { get; set; }
    }
}
