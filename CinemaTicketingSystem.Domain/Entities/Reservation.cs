using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        //public int UserId { get; set; }
        //public ApplicationUser User { get; set; }
        [Required]
        public int ShowTimeId { get; set; }
        [ForeignKey("ShowTimeId")]
        public ShowTime ShowTime { get; set; }
        [Required]
        public int SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }
        [Required]
        public DateOnly ReservationDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string PaymentStatus { get; set; }
    }
}
