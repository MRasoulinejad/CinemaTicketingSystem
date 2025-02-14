﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public string Movie { get; set; }
        public string Theatre { get; set; }
        public string Seat { get; set; }
        public DateOnly ShowDate { get; set; }
    }
}
