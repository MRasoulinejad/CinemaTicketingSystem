using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Repository
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowtimeRepository
    {
        private readonly ApplicationDbContext _db;
        public ShowTimeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShowTime entity)
        {
            _db.Update(entity);
        }

    }
}
