using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ShowTimeSearchForResDto
    {
        public int ShowTimeId { get; set; }
        public string ShowDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Theatre { get; set; }
        public string Hall { get; set; }
        public string Movie { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public int ReservedSeats { get; set; }
    }
}
