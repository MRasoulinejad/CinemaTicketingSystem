using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface ITemporaryReservationRepository
    {
        Task RemoveOlderThanAsync(DateTime cutoffTime, CancellationToken cancellationToken);
    }
}
