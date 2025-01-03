﻿using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
