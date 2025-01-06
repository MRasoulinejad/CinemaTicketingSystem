using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Hall
    {
        [Key]
        public int HallId { get; set; }
        [Required]
        public string HallName { get; set; }
        [Required]
        public int TheatreId { get; set; }
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
