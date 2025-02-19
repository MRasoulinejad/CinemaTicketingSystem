using CinemaTicketingSystem.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IReservationService
    {
        Task<List<ShowTimeSearchForResDto>> SearchReservationByShowTimeAsync(string query, string filterBy);
        Task<List<ReservationSearchDto>> SearchReservationByUserOrTicketAsync(string query, string filterBy);
        Task<MoviesAndTheatresDto> GetMoviesAndTheatresAsync();
        Task<BookShowTimeDto> BookShowTimeAsync(int showTimeId);

        Task<List<FilteredShowTimeDto>> GetFilteredShowTimesAsync(ReservationFilterDto model);
        Task<ProceedBookingSeatDto> ProceedBookingSeatAsync(int showTimeId, int seatCount);

        Task<ConfirmCheckoutResultDto> ConfirmCheckoutAsync(ConfirmCheckoutDto model, string userName);

        Task<CheckoutConfirmationDto> CheckoutConfirmationAsync(int showTimeId, string selectedSeats, string userName);

        Task<FinalizeBookingResultDto> FinalizeBookingAsync(FinalizeBookingDto model, string domain);

        Task<PaymentResultDto> ProcessPaymentSuccessAsync(string sessionId, string userName);
        Task<PaymentResultDto> ProcessPaymentFailedAsync(string sessionId, string userName);

        Task<List<TicketDto>> GetSuccessfulReservationsAsync(string reservationIds);

    }
}
