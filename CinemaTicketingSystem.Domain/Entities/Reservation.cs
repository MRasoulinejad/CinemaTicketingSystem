using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        //public ApplicationUser User { get; set; }
        public int ShowTimeId { get; set; }
        public ShowTime ShowTime { get; set; }
        public int SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }
        public DateOnly ReservationDate { get; set; }
        public String Status { get; set; }
        public string PaymentStatus { get; set; }
    }
}
