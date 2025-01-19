using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface ITemporarySeatReservationRepository : IRepository<TemporarySeatReservation>
    {
        void Update(TemporarySeatReservation entity);
    }
}
