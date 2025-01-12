using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface IShowtimeRepository : IRepository<ShowTime>
    {
        void Update(ShowTime entity);
    }
}
