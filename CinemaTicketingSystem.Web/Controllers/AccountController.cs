using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
        {
            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password, bool RememberMe)
        {

            string Response = Request.Form["g-recaptcha-response"];

            // Validate Google reCAPTCHA
            if (!ValidateReCaptcha(Response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View();
            }

            if (Username == "admin" && Password == "password") // Placeholder logic
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Invalid"] = "Invalid username or password.";
            return View();
        }

        public IActionResult Register()
        {
            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserVM model)
        {
            if (!model.Terms)
            {
                ViewData["Error"] = "You must agree to the terms and conditions.";
                return View();
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewData["Error"] = "Passwords do not match.";
                return View();
            }

            model.gRecaptchaResponse = Request.Form["g-recaptcha-response"];

            // Validate Google reCAPTCHA
            if (!ValidateReCaptcha(model.gRecaptchaResponse))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View();
            }

            ViewData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        private bool ValidateReCaptcha(string response)
        {
            var secret = _configuration["GoogleReCaptcha:SecretKey"];
            var client = new System.Net.Http.HttpClient();
            var result = client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}",
                null).Result;

            var json = result.Content.ReadAsStringAsync().Result;
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            return data.success == true;
        }
    }
}
