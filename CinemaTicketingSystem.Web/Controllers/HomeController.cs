using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.Models;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IReCaptchaValidator _reCaptchaValidator;
        private readonly ISmtpEmailService _emailService;
        private readonly IMovieService _movieService;

        public HomeController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IReCaptchaValidator reCaptchaValidator, ISmtpEmailService emailService, IMovieService movieService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _reCaptchaValidator = reCaptchaValidator;
            _emailService = emailService;
            _movieService = movieService;
        }


        public IActionResult Index()
        {
            // Take 3 Random Theatres
            var theatres = _unitOfWork.Theatres.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();


            // Take 9 Random Movies
            var movies = _unitOfWork.Movies.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(9)
                .ToList();



            // Pass both to the View using a ViewModel
            var HomeVM = new HomeViewModel
            {
                RandomTheatres = theatres,
                LatestMovies = movies
            };

            ViewData["HeroImageUrl"] = "/images/hero-banner.jpg";
            ViewData["HeroTitle"] = "Experience Movies Like Never Before!";
            ViewData["HeroSubtitle"] = "Find your favorite movies and book tickets now.";
            return View(HomeVM);
        }

        public IActionResult Privacy()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["HeroImageUrl"] = "/images/about-hero.jpg";
            ViewData["HeroTitle"] = "About Our Cinema";
            ViewData["HeroSubtitle"] = "Learn more about our state-of-the-art facilities.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];

            ViewData["HeroImageUrl"] = "/images/hero-banner.jpg";
            ViewData["HeroTitle"] = "Contact Us";
            ViewData["HeroSubtitle"] = "Get in touch with us for any queries or feedback.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormVM model)
        {
            string Response = Request.Form["g-recaptcha-response"];

            // Validate Google reCAPTCHA
            if (!_reCaptchaValidator.ValidateReCaptcha(Response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View();
            }

            // Send Email
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin email from configuration
                    string adminEmail = _configuration["SMTP:AdminEmail"];
                    string subject = $"Contact Form Submission: {model.Subject}";
                    string body = $"Message from: {model.Name} ({model.Email})<br/><br/>{model.Message}";
                    await _emailService.SendEmailAsync(adminEmail, subject, body);
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    //Error
                    TempData["ErrorMessage"] = "An error occurred while sending your message. Please try again.";
                    return RedirectToAction("Contact");
                }
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
