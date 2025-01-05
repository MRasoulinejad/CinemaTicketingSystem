using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Repository
{
    public class TheatreRepository : Repository<Theatre>, ITheatreRepository
    {
        private readonly ApplicationDbContext _db;
        public TheatreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Theatre entity)
        {
            _db.Update(entity);
        }
    }
}
