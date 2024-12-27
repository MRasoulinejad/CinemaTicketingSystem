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
        public DateOnly ShowDate { get; set; }
        public TimeSpan ShowTimeStart { get; set; }
        public TimeSpan ShowTimeEnd { get; set; }
        public int TheatreId { get; set; }
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        public decimal Price { get; set; }
    }
}
