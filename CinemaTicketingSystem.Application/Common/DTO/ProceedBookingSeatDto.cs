using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ProceedBookingSeatDto
    {
        public int ShowTimeId { get; set; }
        public int SeatCount { get; set; }
        public string HallName { get; set; }
        public List<SeatDto> Seats { get; set; }
        public List<SectionDto> Sections { get; set; }
    }
}
