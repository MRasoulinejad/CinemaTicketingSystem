﻿using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TemporarySeatReservation> TemporarySeatReservations { get; set; }


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
                    Poster = "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Matrix_Poster.jpg",
                    TrailerUrl = "https://www.youtube.com/embed/m8e-FF8MsqU"
                }
                );


            modelBuilder.Entity<TemporarySeatReservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId);

                // Define relationship with ShowTime
                entity.HasOne(e => e.ShowTime)
                      .WithMany()
                      .HasForeignKey(e => e.ShowTimeId)
                      .OnDelete(DeleteBehavior.NoAction); // Prevent cascading deletes

                // Define relationship with Seat
                entity.HasOne(e => e.Seat)
                      .WithMany()
                      .HasForeignKey(e => e.SeatId)
                      .OnDelete(DeleteBehavior.NoAction); // Prevent cascading deletes
            });
        }

    }
}
