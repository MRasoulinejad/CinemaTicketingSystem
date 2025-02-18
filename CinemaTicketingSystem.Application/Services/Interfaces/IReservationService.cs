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
        Task<List<MovieDto>> GetMoviesAsync();
        Task<List<TheatreDto>> GetTheatresAsync();
        Task<MoviesAndTheatresDto> GetMoviesAndTheatresAsync();

        Task<List<ShowTimeDto>> GetFilteredShowTimesAsync(ReservationFilterDto model);
        Task<ProceedBookingSeatDto> GetSeatDetailsAsync(int showTimeId, int seatCount);
        Task<string> ConfirmCheckoutAsync(ConfirmCheckoutDto model, string userName);
        Task<CheckoutConfirmationDto> GetCheckoutDetailsAsync(int showTimeId, string selectedSeats, string userName);
        Task<string> FinalizeBookingAsync(FinalizeBookingDto model, string domain);
        Task<string> HandlePaymentSuccessAsync(string sessionId, string userName);
        Task<string> HandlePaymentFailedAsync(string sessionId, string userName);
    }
}
