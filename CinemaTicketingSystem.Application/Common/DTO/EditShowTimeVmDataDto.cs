using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class EditShowTimeVmDataDto
    {
        public int ShowTimeId { get; set; }

        public string HallName { get; set; }

        [Required]
        public DateOnly ShowDate { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Set default to today's date
        [Required]
        public TimeSpan ShowTimeStart { get; set; }
        [Required]
        public TimeSpan ShowTimeEnd { get; set; }
        [Required]
        public int TheatreId { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public List<Theatre> Theatres { get; set; }

        public List<Movie> Movies { get; set; }

        public List<Hall> Halls { get; set; }

        public int HallId { get; set; } = 0;
    }
}
