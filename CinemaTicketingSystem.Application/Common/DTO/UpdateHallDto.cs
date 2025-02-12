using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class UpdateHallDto
    {
        public int HallId { get; set; }
        public string HallName { get; set; }
        public List<UpdateSectionDto> Sections { get; set; }
    }
}
