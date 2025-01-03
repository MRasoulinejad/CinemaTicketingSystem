using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class RegisterUserVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Terms { get; set; }
        [ValidateNever]
        public string gRecaptchaResponse { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
