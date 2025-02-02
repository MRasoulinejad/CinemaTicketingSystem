using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class TemporarySeatReservation
    {
        [Key]
        public int ReservationId { get; set; } // Primary Key

        [Required]
        public int ShowTimeId { get; set; } // Foreign Key to ShowTime
        [ForeignKey("ShowTimeId")]
        public ShowTime ShowTime { get; set; } // Navigation Property

        [Required]
        public int SeatId { get; set; } // Foreign Key to Seat
        [ForeignKey("SeatId")]
        public Seat Seat { get; set; } // Navigation Property

        [Required]
        public string UserId { get; set; } // Identity User ID
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public DateTime ReservedAt { get; set; } // Reservation Time
    }
}
