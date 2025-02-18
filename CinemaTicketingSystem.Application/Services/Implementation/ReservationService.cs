using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReservationService(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BookShowTimeDto> BookShowTimeAsync(int showTimeId)
        {
            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            if (showTime == null) return null;

            var movie = _unitOfWork.Movies.Get(x => x.MovieId == showTime.MovieId);
            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);
            var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == showTime.TheatreId);

            return new BookShowTimeDto
            {
                ShowTime = showTime,
                Movie = movie,
                Hall = hall,
                Theatre = theatre
            };

        }

        public async Task<string> ConfirmCheckoutAsync(ConfirmCheckoutDto model, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> FinalizeBookingAsync(FinalizeBookingDto model, string domain)
        {
            throw new NotImplementedException();
        }

        public async Task<CheckoutConfirmationDto> GetCheckoutDetailsAsync(int showTimeId, string selectedSeats, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FilteredShowTimeDto>> GetFilteredShowTimesAsync(ReservationFilterDto filter)
        {
            var query = _unitOfWork.ShowTimes.GetAll(includeProperties: "Movie,Theatre")
                        .Where(st => st.ShowDate == filter.ShowDate);

            if (filter.MovieId.HasValue)
                query = query.Where(st => st.MovieId == filter.MovieId.Value);

            if (filter.TheatreId.HasValue)
                query = query.Where(st => st.TheatreId == filter.TheatreId.Value);

            return query.Select(st => new FilteredShowTimeDto
            {
                ShowTimeId = st.ShowTimeId,
                MovieTitle = st.Movie.Title,
                TheatreName = st.Theatre.TheatreName,
                StartTime = st.ShowTimeStart.ToString(@"hh\:mm"),
                EndTime = st.ShowTimeEnd.ToString(@"hh\:mm"),
                Price = st.Price
            }).ToList();
        }

        public async Task<MoviesAndTheatresDto> GetMoviesAndTheatresAsync()
        {
            var movies = _unitOfWork.Movies.GetAll()
                .Select(m => new MovieDto { MovieId = m.MovieId, Title = m.Title })
                .ToList();

            var theatres = _unitOfWork.Theatres.GetAll()
                .Select(t => new TheatreDto { TheatreId = t.TheatreId, Name = t.TheatreName })
                .ToList();

            return new MoviesAndTheatresDto
            {
                Movies = movies,
                Theatres = theatres
            };
        }



        public async Task<ProceedBookingSeatDto> ProceedBookingSeatAsync(int showTimeId, int seatCount)
        {
            // Fetch the ShowTime entity
            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            if (showTime == null) return null;

            // Fetch the Hall associated with the ShowTime
            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);
            if (hall == null) return null;

            var now = DateTime.UtcNow;

            // Fetch all relevant temporary reservations for the given ShowTime within the last 5 minutes
            var temporaryReservations = _unitOfWork.TemporarySeatReservations.GetAll(r =>
                r.ShowTimeId == showTimeId && r.ReservedAt > now.AddMinutes(-5)).ToList();

            // Fetch all confirmed and paid reservations for the given ShowTime
            var confirmedReservations = _unitOfWork.Reservations.GetAll(r =>
                r.ShowTimeId == showTimeId && r.Status == "Confirmed" && r.PaymentStatus == "Paid").Select(r => r.SeatId).ToHashSet();

            // Fetch and group seats by sections
            var sectionInfo = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
                .GroupBy(s => s.SectionName)
                .Select(group => new
                {
                    SectionName = group.Key,
                    SectionCount = group.Count()
                }).ToList();

            // Fetch seat data, including temporary and confirmed reservation statuses
            var seats = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
                .Select(s => new SeatForProceedBookingDto
                {
                    SeatId = s.SeatId,
                    SectionName = s.SectionName,
                    SeatNumber = s.SeatNumber,
                    IsReserved = confirmedReservations.Contains(s.SeatId), // Check against confirmed reservations
                    IsTemporaryReserved = temporaryReservations.Any(r => r.SeatId == s.SeatId) // Check against temporary reservations
                }).ToList();

            // Populate the DTO
            return new ProceedBookingSeatDto
            {
                ShowTimeId = showTimeId,
                SeatCount = seatCount,
                HallName = hall.HallName,
                Seats = seats,
                Sections = sectionInfo.Select(s => new SectionDto
                {
                    SectionName = s.SectionName,
                    SectionCount = s.SectionCount
                }).ToList()
            };
        }



        public async Task<string> HandlePaymentFailedAsync(string sessionId, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> HandlePaymentSuccessAsync(string sessionId, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ShowTimeSearchForResDto>> SearchReservationByShowTimeAsync(string query, string filterBy)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
                return new List<ShowTimeSearchForResDto>();

            List<int> relevantIds = new List<int>();

            // Filter based on the criteria
            switch (filterBy.ToLower())
            {
                case "movie":
                    // Fetch relevant MovieIds
                    relevantIds = _unitOfWork.Movies
                        .GetAll()
                        .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Select(m => m.MovieId)
                        .ToList();
                    break;

                case "theatre":
                    // Fetch relevant TheatreIds
                    relevantIds = _unitOfWork.Theatres
                        .GetAll()
                        .Where(t => t.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.TheatreId)
                        .ToList();
                    break;

                default:
                    return new List<ShowTimeSearchForResDto>();
            }

            if (!relevantIds.Any())
                return new List<ShowTimeSearchForResDto>();

            // Fetch ShowTimes based on the filtered IDs
            var showTimes = filterBy.ToLower() == "movie"
                ? _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.MovieId))
                : _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.TheatreId));

            return showTimes.Select(s => new ShowTimeSearchForResDto
            {
                ShowTimeId = s.ShowTimeId,
                ShowDate = s.ShowDate.ToShortDateString(),
                StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
                EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
                Theatre = _unitOfWork.Theatres.Get(t => t.TheatreId == s.TheatreId)?.TheatreName ?? "N/A",
                Hall = _unitOfWork.Halls.Get(h => h.HallId == s.HallId)?.HallName ?? "N/A",
                Movie = _unitOfWork.Movies.Get(m => m.MovieId == s.MovieId)?.Title ?? "N/A",
                Price = s.Price,
                TotalSeats = _unitOfWork.Seats.GetAll().Count(seat => seat.HallId == s.HallId),
                ReservedSeats = _unitOfWork.Reservations.GetAll()
                    .Count(r => r.ShowTimeId == s.ShowTimeId && r.PaymentStatus == SD.PaymentStatus_Paid)
            }).ToList();
        }

        public async Task<List<ReservationSearchDto>> SearchReservationByUserOrTicketAsync(string query, string filterBy)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
                return new List<ReservationSearchDto>();

            ApplicationUser user = null;
            int ticketId = 0;

            switch (filterBy.ToLower())
            {
                case "user":
                    user = _userManager.Users.FirstOrDefault(u => u.Email == query.ToLower());
                    if (user == null) return new List<ReservationSearchDto>(); // Return empty list if user not found
                    break;

                case "ticket":
                    if (query.Contains("@")) return new List<ReservationSearchDto>(); // Prevent searching by email for ticket

                    if (!int.TryParse(query, out ticketId)) return new List<ReservationSearchDto>(); // Ensure it's a valid number

                    var reservation = _unitOfWork.Reservations.Get(r => r.ReservationId == ticketId);
                    if (reservation == null) return new List<ReservationSearchDto>();

                    // Retrieve the user associated with the reservation
                    user = _userManager.Users.FirstOrDefault(u => u.Id == reservation.UserId);
                    break;

                default:
                    return new List<ReservationSearchDto>(); // Invalid filter value
            }
            // Fetch reservations based on the user or ticket ID
            var reservations = filterBy.ToLower() == "user"
                ? _unitOfWork.Reservations.GetAll(r => r.UserId == user.Id,
                includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat")
                : _unitOfWork.Reservations.GetAll(r => r.ReservationId == ticketId,
                includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat");

            return reservations
                .OrderByDescending(r => r.ReservationId)
                .Select(r => new ReservationSearchDto
                {
                    TicketNumber = r.ReservationId,
                    UserEmail = user.Email,
                    Movie = r.ShowTime.Movie.Title,
                    Theatre = r.ShowTime.Theatre.TheatreName,
                    Hall = _unitOfWork.Halls.Get(h => h.HallId == r.ShowTime.HallId)?.HallName ?? "N/A",
                    ShowDate = r.ShowTime.ShowDate.ToShortDateString(),
                    SeatNumber = $"{r.Seat.SectionName} {r.Seat.SeatNumber}",
                    Status = r.Status,
                    PaymentStatus = r.PaymentStatus
                }).ToList();
        }
    }
}
