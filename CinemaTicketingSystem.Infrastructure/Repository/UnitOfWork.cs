using CinemaTicketingSystem.Application.Common.Interfaces;
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
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Movies = new MovieRepository(_db);
        }
    }
}
