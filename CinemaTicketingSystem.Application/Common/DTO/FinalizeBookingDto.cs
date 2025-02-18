using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class FinalizeBookingDto
    {
        public int ShowTimeId { get; set; }
        public List<int> SelectedSeatIds { get; set; }
    }
}
