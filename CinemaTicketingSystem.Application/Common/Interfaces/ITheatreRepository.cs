using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface ITheatreRepository : IRepository<Theatre>
    {
        void Update(Theatre entity);
    }
}
