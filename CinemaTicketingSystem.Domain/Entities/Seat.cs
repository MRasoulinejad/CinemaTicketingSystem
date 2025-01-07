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
        public string SectionName { get; set; }
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        public bool IsReserved { get; set; }
        [Required]
        public int HallId { get; set; }
        [ForeignKey("HallId")]
        public Hall Hall { get; set; }
    }
}
