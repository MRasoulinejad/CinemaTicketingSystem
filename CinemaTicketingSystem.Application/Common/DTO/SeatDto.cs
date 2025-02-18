using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class SeatDto
    {
        public int SeatId { get; set; }
        public string SectionName { get; set; }
        public string SeatNumber { get; set; }
        public bool IsReserved { get; set; }
        public bool IsTemporaryReserved { get; set; }
    }
}
