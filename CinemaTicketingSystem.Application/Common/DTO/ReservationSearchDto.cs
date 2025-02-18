using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ReservationSearchDto
    {
        public int TicketNumber { get; set; }
        public string UserEmail { get; set; }
        public string Movie { get; set; }
        public string Theatre { get; set; }
        public string Hall { get; set; }
        public string ShowDate { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
    }
}
