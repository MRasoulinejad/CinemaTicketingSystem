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
    public class HallRepository : Repository<Hall>, IHallRepository
    {
        private readonly ApplicationDbContext _db;
        public HallRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Hall entity)
        {
            _db.Update(entity);
        }

    }
}
