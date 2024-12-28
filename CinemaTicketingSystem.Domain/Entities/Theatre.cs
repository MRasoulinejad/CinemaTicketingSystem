﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Theatre
    {
        [Key]
        public int TheatreId { get; set; }
        [Required]
        public string TheatreName { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        [Required]
        public string Location { get; set; }
        public string? ImageUrl { get; set; }
    }
}
