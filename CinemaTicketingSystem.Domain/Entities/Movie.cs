using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int Duration { get; set; }
        [Required]
        public DateOnly ReleaseDate { get; set; }
        [Required]
        public string Poster { get; set; }
    }
}
