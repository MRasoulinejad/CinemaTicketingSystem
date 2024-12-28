using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class ShowTime
    {
        [Key]
        public int ShowTimeId { get; set; }
        [Required]
        public DateOnly ShowDate { get; set; }
        [Required]
        public TimeSpan ShowTimeStart { get; set; }
        [Required]
        public TimeSpan ShowTimeEnd { get; set; }
        [Required]
        public int TheatreId { get; set; }
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
        [Required]
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}
