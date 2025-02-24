using CinemaTicketingSystem.Application.Common.DTO;
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
        private readonly IHomeService _homeService;

        public HomeController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IReCaptchaValidator reCaptchaValidator, ISmtpEmailService emailService, IMovieService movieService, IHomeService homeService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _reCaptchaValidator = reCaptchaValidator;
            _emailService = emailService;
            _movieService = movieService;
            _homeService = homeService;
        }


        public async Task<IActionResult> Index()
        {

            // Get Data for Home Page
            var HomeData = await _homeService.GetDataForIndexAsync();

            //// Take 3 Random Theatres
            //var theatres = _unitOfWork.Theatres.GetAll()
            //    .OrderBy(x => Guid.NewGuid())
            //    .Take(3)
            //    .ToList();

            //// Take 9 Random Movies
            //var movies = _unitOfWork.Movies.GetAll()
            //    .OrderBy(x => Guid.NewGuid())
            //    .Take(9)
            //    .ToList();

            // Pass both to the View using a ViewModel
            var HomeVM = new HomeViewModel
            {
                RandomTheatres = HomeData.RandomTheatres,
                LatestMovies = HomeData.LatestMovies
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

            string response = Request.Form["g-recaptcha-response"];

            if (string.IsNullOrEmpty(response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["ErrorMessage"] = "Please complete the reCAPTCHA.";
                return View(model);
            }


            // Validate Google reCAPTCHA
            if (!_reCaptchaValidator.ValidateReCaptcha(response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["ErrorMessage"] = "Invalid reCAPTCHA. Please try again.";
                return View(model);
            }
            

            // if model is not valid
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all the required fields.";
                return View(model);

            }
            //Send Email
            try
            {
                    // Get the admin email from configuration
                    string adminEmail = _configuration["SMTP:AdminEmail"];

                    // Send Email
                    bool result = await _homeService.SendContactEmailAsync(new ContactFormVmDto
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Subject = model.Subject,
                        Message = model.Message,
                        AdminEmail = adminEmail
                    });

                    if (result)
                    {
                        //Success
                        TempData["SuccessMessage"] = "Your message has been sent successfully!";
                        return View();
                }
                    else
                    {
                        //Error
                        TempData["ErrorMessage"] = "An error occurred while sending your message. Please try again.";
                        return View(model);
                }
                    
                }
                catch (Exception ex)
                {
                    //Error
                    TempData["ErrorMessage"] = ex.Message;
                    return View(model);
            }
            
        }

        //response should not be cached at all & neither the client nor any intermediate servers should cache the response & browsers not to store any version of this page in their cache
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
