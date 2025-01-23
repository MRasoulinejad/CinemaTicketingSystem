using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IMovieRepository Movies { get; }
        ITheatreRepository Theatres { get; }
        IHallRepository Halls { get; }
        ISeatRepository Seats { get; }
        IShowtimeRepository ShowTimes { get; }
        ITemporarySeatReservationRepository TemporarySeatReservations { get; }
        IReservationRepository Reservations { get; }
        void Save();
    }
}
