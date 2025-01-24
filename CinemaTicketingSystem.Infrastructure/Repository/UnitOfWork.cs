using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IMovieRepository Movies { get; private set; }

        public ITheatreRepository Theatres { get; private set; }

        public IHallRepository Halls { get; private set; }

        public ISeatRepository Seats { get; private set; }

        public IShowtimeRepository ShowTimes { get; private set; }

        public ITemporarySeatReservationRepository TemporarySeatReservations { get; private set; }

        public IReservationRepository Reservations { get; private set; }

        public IPaymentRepository Payments { get; private set; }


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Movies = new MovieRepository(_db);
            Theatres = new TheatreRepository(_db);
            Halls = new HallRepository(_db);
            Seats = new SeatRepository(_db);
            ShowTimes = new ShowTimeRepository(_db);
            TemporarySeatReservations = new TemporarySeatReservationRepository(_db);
            Reservations = new ReservationRepository(_db);
            Payments = new PaymentRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
