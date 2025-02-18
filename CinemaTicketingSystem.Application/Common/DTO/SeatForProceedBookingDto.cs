using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class SeatForProceedBookingDto
    {
        public int SeatId { get; set; }
        public string SectionName { get; set; }
        public int SeatNumber { get; set; }
        public bool IsReserved { get; set; } // Permanently reserved
        public bool IsTemporaryReserved { get; set; } // Temporarily reserved
    }
}
