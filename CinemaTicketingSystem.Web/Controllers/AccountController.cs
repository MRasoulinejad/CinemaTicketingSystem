using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Services;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System.Data;
using System.Text.Encodings.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmtpEmailService _smtpEmailService;

        public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork,
            ISmtpEmailService smtpEmailService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _smtpEmailService = smtpEmailService;
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

                if (result.Succeeded)
                {
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


        public IActionResult AdminUserManager()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Query cannot be empty." });
            }

            try
            {
                // Fetch users that match the query
                var users = _userManager.Users
                    .Where(u => u.Email.Contains(query.ToLower()) ||
                                u.FirstName.Contains(query.ToLower()) ||
                                u.LastName.Contains(query.ToLower()))
                    .ToList();

                // Fetch roles for each user
                var userWithRoles = new List<object>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user); // Fetch roles asynchronously
                    userWithRoles.Add(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        Roles = roles
                    });
                }

                return Json(userWithRoles);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User Email cannot be empty.");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // Create a ViewModel to send data to the view
            var userViewModel = new UpdateUserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.FirstOrDefault(),
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList() // Get all available roles
            };

            return View(userViewModel); // Pass the ViewModel to the view
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserSecondStep(UpdateUserVM model)
        {
            // Find the user by email (or use Id for a more reliable approach)
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound("User not found");
            }


            // Check if the email already exists and belongs to another user
            var emailExists = await _userManager.Users
                .AnyAsync(u => u.Email == model.Email && u.Id != model.Id);

            if (emailExists)
            {
                TempData["error"] = "The provided email address is already in use by another user.";
                return RedirectToAction("AdminUserManager");
            }


            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email; // Ensure this doesn't conflict with another user's email
            user.PhoneNumber = model.PhoneNumber;

            // Update user in database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest("Failed to update user details");
            }

            // Update roles if different
            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = model.Roles; // Assuming a single role from the model
            if (!string.IsNullOrEmpty(newRole) && !currentRoles.Contains(newRole))
            {
                // Remove existing roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest("Failed to remove existing roles");
                }

                // Add the new role
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    return BadRequest("Failed to assign the new role");
                }
            }
            TempData["success"] = "User data successfully updated";
            return RedirectToAction("AdminUserManager");
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var userName = User.Identity.Name;

            if (userName == null) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) return NotFound("User not found");

            var reservations = _unitOfWork.Reservations.GetAll(r => r.UserId == user.Id,
                includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat").ToList();

            var model = new MyAccountVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Reservations = reservations
            };

            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with the provided email address.");
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token },
                protocol: HttpContext.Request.Scheme);


            // Create email template
            var emailBody = $@"
            <h2>Reset Your Password</h2>
            <p>To reset your password, please click the link below:</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a></p>
            <p>If you didn't request a password reset, please ignore this email.</p>";

            // Send email with the reset link
            await _smtpEmailService.SendEmailAsync(
                model.Email,
                "Password Reset Request",
                emailBody);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError("", "Invalid password reset token.");
                return View();
            }
            var model = new ResetPasswordVM
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with the provided email address.");
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

    
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
