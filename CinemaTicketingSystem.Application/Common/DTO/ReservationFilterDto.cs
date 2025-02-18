using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ReservationFilterDto
    {
        public int? MovieId { get; set; }
        public int? TheatreId { get; set; }
        public DateTime ShowDate { get; set; }
    }
}
