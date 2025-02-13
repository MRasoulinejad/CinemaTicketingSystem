using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ShowTimeManagementDto
    {
        public List<ShowTime> ShowTimes { get; set; }
        public int TotalShowTimes { get; set; }
        public int TotalTheatres { get; set; }
        public int TotalMovies { get; set; }
        public double TotalMinutes { get; set; }
    }
}
