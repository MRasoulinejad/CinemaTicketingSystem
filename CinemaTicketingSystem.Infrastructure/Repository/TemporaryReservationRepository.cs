using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Repository
{
    public class TemporaryReservationRepository : ITemporaryReservationRepository
    {
        private readonly ApplicationDbContext _db;

        public TemporaryReservationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task RemoveOlderThanAsync(DateTime cutoffTime, CancellationToken cancellationToken)
        {
            var oldReservations = await _db.TemporarySeatReservations
                .Where(r => r.ReservedAt < cutoffTime)
                .ToListAsync(cancellationToken);

            _db.TemporarySeatReservations.RemoveRange(oldReservations);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
