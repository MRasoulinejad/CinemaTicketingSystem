using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class AddShowTimeViewDataDto
    {
        public List<Theatre> Theatres { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
