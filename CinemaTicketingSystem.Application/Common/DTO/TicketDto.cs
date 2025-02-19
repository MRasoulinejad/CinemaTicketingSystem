using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public string HallName { get; set; }
        public string SeatNumber { get; set; }
        public string ShowDate { get; set; }
        public string ShowTime { get; set; }
        public decimal Price { get; set; }
    }
}
