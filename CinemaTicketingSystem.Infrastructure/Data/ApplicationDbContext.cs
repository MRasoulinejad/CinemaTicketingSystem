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
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ShowTime)
                .WithMany()
                .HasForeignKey(r => r.ShowTimeId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Seat)
                .WithMany()
                .HasForeignKey(r => r.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
                .HasData(
                new Movie
                {
                    MovieId = 1,
                    Title = "The Matrix",
                    Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                    Genre = "Action, Sci-Fi",
                    Duration = 136,
                    ReleaseDate = new DateOnly(1999, 3, 1),
                    Poster = "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Matrix_Poster.jpg"
                }
                );
        }





    }
}
