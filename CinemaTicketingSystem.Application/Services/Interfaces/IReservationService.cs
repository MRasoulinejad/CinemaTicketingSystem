using CinemaTicketingSystem.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<string> ConfirmCheckoutAsync(ConfirmCheckoutDto model, string userName);
        Task<CheckoutConfirmationDto> GetCheckoutDetailsAsync(int showTimeId, string selectedSeats, string userName);
        Task<string> FinalizeBookingAsync(FinalizeBookingDto model, string domain);
        Task<string> HandlePaymentSuccessAsync(string sessionId, string userName);
        Task<string> HandlePaymentFailedAsync(string sessionId, string userName);
    }
}
