using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class MovieListDto
    {
        public List<MovieDto> Movies { get; set; }
        public int Skip { get; set; }
        public bool HasMore { get; set; }
    }
}
