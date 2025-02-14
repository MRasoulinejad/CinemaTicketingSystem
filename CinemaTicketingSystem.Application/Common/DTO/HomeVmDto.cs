using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class HomeVmDto
    {
        public List<Theatre> RandomTheatres { get; set; }
        public List<Movie> LatestMovies { get; set; }
    }
}
