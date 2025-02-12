using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class AddSectionDto
    {
        public int HallId { get; set; }
        public string SectionName { get; set; }
        public int NumberOfSeats { get; set; }
    }
}
