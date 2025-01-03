using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Login()
        {
            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {

            string Response = Request.Form["g-recaptcha-response"];

            // Validate Google reCAPTCHA
            if (!ValidateReCaptcha(Response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View();
            }

            if (ModelState.IsValid)
            {

                var result = _signInManager
                    .PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false).Result;

                if (result.Succeeded) {
                    var user = await _userManager.FindByEmailAsync(model.Username);
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(model.RedirectUrl);
                        }
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
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
        public async Task<IActionResult> Register(RegisterUserVM model)
        {
            model.gRecaptchaResponse = Request.Form["g-recaptcha-response"];

            // Validate Google reCAPTCHA
            if (!ValidateReCaptcha(model.gRecaptchaResponse))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View();
            }

            if (ModelState.IsValid)
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

                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.Now
                };

                ApplicationUser userInDB = await _userManager.FindByEmailAsync(model.Email);
                if (userInDB == null)
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        if (string.IsNullOrEmpty(model.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(model.RedirectUrl);
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View();
                    }
                } 
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

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
