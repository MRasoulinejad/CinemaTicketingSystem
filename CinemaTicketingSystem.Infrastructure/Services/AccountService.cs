using Azure.Core;
using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System;
using System.Buffers.Text;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class AccountService : IAccountService
    {

        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork,
            ISmtpEmailService smtpEmailService,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _smtpEmailService = smtpEmailService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> LoginAsync(LoginDto model)
        {

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                return user != null && await _userManager.IsInRoleAsync(user, SD.Role_Admin)
                    ? "/Home/Index"
                    : model.RedirectUrl ?? "/Home/Index";
            }
            return "Invalid login attempt";

        }


        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return "User not found";
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Generate the password reset link
            var baseUrl = _configuration["ApplicationSettings:BaseUrl"];
            var callbackUrl = $"{baseUrl}/Account/ResetPassword?email={user.Email}&token={token}";

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

            return "Success";

        }

        public async Task<MyAccountDto> GetMyAccountAsync(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return null;
            }
            var reservations = _unitOfWork.Reservations.GetAll(r => r.UserId == user.Id,
                includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat").ToList();

            return new MyAccountDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Reservations = reservations
            };
        }

        public async Task<UpdateUserDto> GetUserDetailsAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.FirstOrDefault(),
                AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync()
            };
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> RegisterAsync(RegisterUserDto model)
        {
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

            if (userInDB != null)
            {
                return "User already exists";
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return "Success";
            }

            return "Failed";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return "User not found";
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return "Success";
            }

            return "Failed";
        }

        public async Task<List<UserWithRolesDto>> SearchUsersAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<UserWithRolesDto>();

            var users = await _userManager.Users
                .Where(u => u.Email.ToLower().Contains(query.ToLower()) ||
                            u.FirstName.ToLower().Contains(query.ToLower()) ||
                            u.LastName.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            var userWithRolesList = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRolesList.Add(new UserWithRolesDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return userWithRolesList;
        }

        public async Task<string> UpdateUserAsync(UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return "User not found.";

            var emailExists = await _userManager.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.Id);
            if (emailExists)
                return "The provided email address is already in use by another user.";

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email; // Ensure this doesn't conflict with another user's email
            user.PhoneNumber = model.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return "Failed to update user details.";

            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = model.Roles; // Assuming a single role from the model
            if (!string.IsNullOrEmpty(newRole) && !currentRoles.Contains(newRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                    return "Failed to assign the new role.";
            }

            return "User data successfully updated";
        }

        public async Task<bool> ValidateReCaptchaAsync(string response)
        {
            var secret = _configuration["GoogleReCaptcha:SecretKey"];
            var client = _httpClientFactory.CreateClient();
            var result = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}",
                null);

            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);
            return data.success == true;
        }
    }
}
