using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int ReservationId { get; set; }
        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [Required]
        public DateOnly PaymentDate { get; set; }
        [Required]
        public string PaymentStatus { get; set; }

        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
    }
}
