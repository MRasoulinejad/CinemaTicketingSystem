using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class UpdateUserVM
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; } // Current roles of the user
        public List<string> AllRoles { get; set; } // All available roles
    }
}
