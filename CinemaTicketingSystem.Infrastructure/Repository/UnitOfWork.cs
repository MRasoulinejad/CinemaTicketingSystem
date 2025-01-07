﻿using CinemaTicketingSystem.Application.Common.Interfaces;
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

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Movies = new MovieRepository(_db);
            Theatres = new TheatreRepository(_db);
            Halls = new HallRepository(_db);
            Seats = new SeatRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
