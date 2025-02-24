using Azure;
using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Services.Interfaces;
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
        private readonly IAccountService _accountService;

        public AccountController(IConfiguration configuration,
            IAccountService accountService)
        {
            _configuration = configuration;
            _accountService = accountService;
        }

        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new LoginVM
            {
                RedirectUrl = returnUrl
            };

            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];

            return View(loginVM);
        }

        public async Task<IActionResult> AccessDenied() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {

            //string Response = Request.Form["g-recaptcha-response"];

            //// Validate Google reCAPTCHA
            //if (!ValidateReCaptcha(Response))
            //{
            //    ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
            //    ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
            //    return View();
            //}

            //if (ModelState.IsValid)
            //{

            //    var result = _signInManager
            //        .PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false).Result;

            //    if (result.Succeeded)
            //    {
            //        var user = await _userManager.FindByEmailAsync(model.Username);
            //        if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //        else
            //        {
            //            if (string.IsNullOrEmpty(model.RedirectUrl))
            //            {
            //                return RedirectToAction("Index", "Home");
            //            }
            //            else
            //            {
            //                return LocalRedirect(model.RedirectUrl);
            //            }
            //        }

            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Invalid login attempt");
            //    }
            //}

            //ViewData["Invalid"] = "Invalid username or password.";
            //return View();


            string response = Request.Form["g-recaptcha-response"];

            if (string.IsNullOrEmpty(response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["Error"] = "Please complete the reCAPTCHA.";
                return View(model);
            }

            // Validate Google reCAPTCHA
            if (!await _accountService.ValidateReCaptchaAsync(response))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View(model);
            }

            var newModel = new LoginDto
            {
                Username = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe,
                RedirectUrl = model.RedirectUrl
            };

            var result = await _accountService.LoginAsync(newModel);
            if (result == "Invalid login attempt")
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["Error"] = "Invalid login attempt";
                return View(model);
            }

            return Redirect(result);
        }

        public IActionResult Register(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            RegisterUserVM registerUserVM = new RegisterUserVM
            {
                RedirectUrl = returnUrl
            };

            ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];

            return View(registerUserVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM model)
        {
            model.gRecaptchaResponse = Request.Form["g-recaptcha-response"];

            // Check if reCAPTCHA is empty
            if (string.IsNullOrEmpty(model.gRecaptchaResponse))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                TempData["Error"] = "Please complete the reCAPTCHA.";
                return View(model);
            }

            // Validate Google reCAPTCHA
            if (!await _accountService.ValidateReCaptchaAsync(model.gRecaptchaResponse))
            {
                ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                ViewData["Error"] = "Invalid reCAPTCHA. Please try again.";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                if (!model.Terms)
                {
                    ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                    TempData["Error"] = "You must agree to the terms and conditions.";
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                    TempData["Error"] = "Passwords do not match.";
                    return View(model);
                }

                var newModel = new RegisterUserDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Password = model.Password,
                };

                var result = await _accountService.RegisterAsync(newModel);
                if (result == "User already exists" || result == "Failed")
                {
                    ViewData["SiteKey"] = _configuration["GoogleReCaptcha:SiteKey"];
                    TempData["Error"] = result;
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.RedirectUrl))
                {
                    TempData["Success"] = "Registration successful! Please log in.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(model.RedirectUrl);
                }

                //ApplicationUser user = new ApplicationUser
                //{
                //    UserName = model.Email,
                //    Email = model.Email,
                //    EmailConfirmed = true,
                //    FirstName = model.FirstName,
                //    LastName = model.LastName,
                //    PhoneNumber = model.PhoneNumber,
                //    CreatedDate = DateTime.Now
                //};

                //ApplicationUser userInDB = await _userManager.FindByEmailAsync(model.Email);
                //if (userInDB == null)
                //{

                //    var result = await _userManager.CreateAsync(user, model.Password);

                //    if (result.Succeeded)
                //    {
                //        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                //        await _signInManager.SignInAsync(user, isPersistent: false);

                //        if (string.IsNullOrEmpty(model.RedirectUrl))
                //        {
                //            return RedirectToAction("Index", "Home");
                //        }
                //        else
                //        {
                //            return LocalRedirect(model.RedirectUrl);
                //        }
                //    }
                //    else
                //    {
                //        foreach (var error in result.Errors)
                //        {
                //            ModelState.AddModelError("", error.Description);
                //        }
                //        return View();
                //    }
                //}
            }

            ViewData["Error"] = "Registration Failed! Try agin!";
            return RedirectToAction(nameof(Register));
        }

        //private bool ValidateReCaptcha(string response)
        //{
        //    var secret = _configuration["GoogleReCaptcha:SecretKey"];
        //    var client = new System.Net.Http.HttpClient();
        //    var result = client.PostAsync(
        //        $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}",
        //        null).Result;

        //    var json = result.Content.ReadAsStringAsync().Result;
        //    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        //    return data.success == true;
        //}

        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult AdminUserManager()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Query cannot be empty." });
            }

            try
            {
                //// Fetch users that match the query
                //var users = _userManager.Users
                //    .Where(u => u.Email.Contains(query.ToLower()) ||
                //                u.FirstName.Contains(query.ToLower()) ||
                //                u.LastName.Contains(query.ToLower()))
                //    .ToList();

                //// Fetch roles for each user
                //var userWithRoles = new List<object>();
                //foreach (var user in users)
                //{
                //    var roles = await _userManager.GetRolesAsync(user); // Fetch roles asynchronously
                //    userWithRoles.Add(new
                //    {
                //        user.Id,
                //        user.FirstName,
                //        user.LastName,
                //        user.Email,
                //        Roles = roles
                //    });
                //}

                var userWithRoles = await _accountService.SearchUsersAsync(query);

                return Json(userWithRoles);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(string userEmail)
        {
            //if (string.IsNullOrWhiteSpace(userEmail))
            //{
            //    return BadRequest("User Email cannot be empty.");
            //}

            //var user = await _userManager.FindByEmailAsync(userEmail);
            //if (user == null)
            //{
            //    return NotFound("User not found.");
            //}

            //// Retrieve roles for the user
            //var roles = await _userManager.GetRolesAsync(user);

            //// Create a ViewModel to send data to the view
            //var userViewModel = new UpdateUserVM
            //{
            //    Id = user.Id,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    PhoneNumber = user.PhoneNumber,
            //    Roles = roles.FirstOrDefault(),
            //    AllRoles = _roleManager.Roles.Select(r => r.Name).ToList() // Get all available roles
            //};

            //return View(userViewModel); // Pass the ViewModel to the view

            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest("User Email cannot be empty.");

            var userDetails = await _accountService.GetUserDetailsAsync(userEmail);
            if (userDetails == null)
                return NotFound("User not found.");

            var model = new UpdateUserVM
            {
                Id = userDetails.Id,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                PhoneNumber = userDetails.PhoneNumber,
                Roles = userDetails.Roles,
                AllRoles = userDetails.AllRoles
            };

            return View(model);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> UpdateUserSecondStep(UpdateUserVM model)
        {
            //// Find the user by email (or use Id for a more reliable approach)
            //ApplicationUser user = await _userManager.FindByIdAsync(model.Id);

            //if (user == null)
            //{
            //    return NotFound("User not found");
            //}


            //// Check if the email already exists and belongs to another user
            //var emailExists = await _userManager.Users
            //    .AnyAsync(u => u.Email == model.Email && u.Id != model.Id);

            //if (emailExists)
            //{
            //    TempData["error"] = "The provided email address is already in use by another user.";
            //    return RedirectToAction("AdminUserManager");
            //}


            //// Update user properties
            //user.FirstName = model.FirstName;
            //user.LastName = model.LastName;
            //user.Email = model.Email; // Ensure this doesn't conflict with another user's email
            //user.PhoneNumber = model.PhoneNumber;

            //// Update user in database
            //var updateResult = await _userManager.UpdateAsync(user);
            //if (!updateResult.Succeeded)
            //{
            //    return BadRequest("Failed to update user details");
            //}

            //// Update roles if different
            //var currentRoles = await _userManager.GetRolesAsync(user);
            //var newRole = model.Roles; // Assuming a single role from the model
            //if (!string.IsNullOrEmpty(newRole) && !currentRoles.Contains(newRole))
            //{
            //    // Remove existing roles
            //    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            //    if (!removeResult.Succeeded)
            //    {
            //        return BadRequest("Failed to remove existing roles");
            //    }

            //    // Add the new role
            //    var addResult = await _userManager.AddToRoleAsync(user, newRole);
            //    if (!addResult.Succeeded)
            //    {
            //        return BadRequest("Failed to assign the new role");
            //    }
            //}
            //TempData["success"] = "User data successfully updated";
            //return RedirectToAction("AdminUserManager");

            var modelDto = new UpdateUserDto
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Roles = model.Roles,
                AllRoles = model.AllRoles
            };

            var result = await _accountService.UpdateUserAsync(modelDto);
            if (result != "User data successfully updated")
            {
                TempData["error"] = result;
                return RedirectToAction("AdminUserManager");
            }

            TempData["success"] = result;
            return RedirectToAction("AdminUserManager");
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName) ) {
                return RedirectToAction("Login", "Account");
            }

            var accountDetails = await _accountService.GetMyAccountAsync(userName);
            if (accountDetails == null) {
                return NotFound("User not found");
            }

            var model = new MyAccountVM
            {
                UserId = accountDetails.UserId,
                FirstName = accountDetails.FirstName,
                LastName = accountDetails.LastName,
                Email = accountDetails.Email,
                PhoneNumber = accountDetails.PhoneNumber,
                Reservations = accountDetails.Reservations
            };

            return View(model);



            //if (userName == null) return RedirectToAction("Login", "Account");

            //var user = await _userManager.FindByNameAsync(userName);

            //if (user == null) return NotFound("User not found");

            //var reservations = _unitOfWork.Reservations.GetAll(r => r.UserId == user.Id,
            //    includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat").ToList();

            //var model = new MyAccountVM
            //{
            //    UserId = user.Id,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    PhoneNumber = user.PhoneNumber,
            //    Reservations = reservations
            //};

            //return View(model);
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

            var modelDto = new ForgotPasswordDto
            {
                Email = model.Email
            };

            var result = await _accountService.ForgotPasswordAsync(modelDto);

            if (result == "Failed")
            {
                TempData["Error"] = "Failed to send password reset email.";
                return View();
            }

            if (result == "User not found")
            {
                TempData["Error"] = "No user found with the provided email address.";
                return View();
            }


            //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token },
            //    protocol: HttpContext.Request.Scheme);


            //// Create email template
            //var emailBody = $@"
            //<h2>Reset Your Password</h2>
            //<p>To reset your password, please click the link below:</p>
            //<p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a></p>
            //<p>If you didn't request a password reset, please ignore this email.</p>";

            //// Send email with the reset link
            //await _smtpEmailService.SendEmailAsync(
            //    model.Email,
            //    "Password Reset Request",
            //    emailBody);


            TempData["Success"] = "Password reset email sent successfully.";
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
                TempData["Error"] = "Invalid password reset token.";
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

            var modelDto = new ResetPasswordDto
            {
                Email = model.Email,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword,
                Token = model.Token
            };

            var result = await _accountService.ResetPasswordAsync(modelDto);

            if (result == "Failed")
            {
                TempData["Error"] = "Failed to reset password.";
                return View();
            }

            if (result == "User not found")
            {
                TempData["Error"] = "No user";
                return View();
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));


            //var user = await _userManager.FindByEmailAsync(model.Email);
            //if (user == null)
            //{
            //    ModelState.AddModelError("", "No user found with the provided email address.");
            //    return View();
            //}
            //var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction(nameof(ResetPasswordConfirmation));
            //}
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError("", error.Description);
            //}
            
        }

    
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
