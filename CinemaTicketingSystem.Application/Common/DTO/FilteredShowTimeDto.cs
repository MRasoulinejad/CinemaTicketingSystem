using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class FilteredShowTimeDto
    {
        public int ShowTimeId { get; set; }
        public string MovieTitle { get; set; }
        public string TheatreName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Price { get; set; }
    }
}
