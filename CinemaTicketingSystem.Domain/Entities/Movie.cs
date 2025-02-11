using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CinemaTicketingSystem.Domain.Entities
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "The Description cannot exceed 1000 characters.")]
        public string Description { get; set; }
        [Required]
        [Range(1, 300, ErrorMessage = "The Duration must be between 1 and 500 minutes.")]
        public int Duration { get; set; }
        [Required]
        public DateOnly ReleaseDate { get; set; }
        [Required]
        public string Poster { get; set; }
        [Required]
        public string TrailerUrl { get; set; }

    }
}
