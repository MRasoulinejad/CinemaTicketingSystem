using CinemaTicketingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
    }
}
