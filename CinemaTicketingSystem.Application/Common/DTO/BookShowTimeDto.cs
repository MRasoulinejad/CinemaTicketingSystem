using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class BookShowTimeDto
    {
        public ShowTime ShowTime { get; set; }
        public Movie Movie { get; set; }
        public Hall Hall { get; set; }
        public Theatre Theatre { get; set; }
    }
}
