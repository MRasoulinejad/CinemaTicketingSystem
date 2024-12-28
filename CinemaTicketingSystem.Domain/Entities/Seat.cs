using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }
        [Required]
        public string SeatNumber { get; set; }
        [Required]
        public int TheatreId { get; set; }
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
    }
}
