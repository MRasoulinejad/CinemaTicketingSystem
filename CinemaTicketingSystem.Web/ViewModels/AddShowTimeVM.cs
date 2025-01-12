using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class AddShowTimeVM
    {
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

        [ValidateNever]
        public List<Theatre> Theatres { get; set; }

        [ValidateNever]
        public List<Movie> Movies { get; set; }

        [ValidateNever]
        public List<Hall> Halls { get; set; } 

        [ValidateNever]
        public int HallId { get; set; } = 0;
    }
}
