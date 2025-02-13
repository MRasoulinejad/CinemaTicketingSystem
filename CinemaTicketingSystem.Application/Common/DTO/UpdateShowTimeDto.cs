using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class UpdateShowTimeDto
    {
        public int ShowTimeId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeSpan ShowTimeStart { get; set; }
        public TimeSpan ShowTimeEnd { get; set; }
        public int TheatreId { get; set; }
        public int HallId { get; set; }
        public int MovieId { get; set; }
        public decimal Price { get; set; }
    }
}
