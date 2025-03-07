﻿using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces.Payments;
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
        private readonly IStripeService _stripeService;
        public ReservationService(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IStripeService stripeService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _stripeService = stripeService;
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

        public async Task<ConfirmCheckoutResultDto> ConfirmCheckoutAsync(ConfirmCheckoutDto model, string userName)
        {
            try
            {
                // Validate input
                if (model == null || model.SelectedSeatIds == null || !model.SelectedSeatIds.Any())
                {
                    return new ConfirmCheckoutResultDto { Success = false, Message = "Invalid input. Please select at least one seat." };
                }

                // Get the current user from Identity
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return new ConfirmCheckoutResultDto { Success = false, Message = "User not authenticated." };
                }

                // Get current time
                var now = DateTime.UtcNow;

                // Check if any selected seat is already reserved
                var reservedSeats = _unitOfWork.TemporarySeatReservations.GetAll(r =>
                    r.ShowTimeId == model.ShowTimeId &&
                    model.SelectedSeatIds.Contains(r.SeatId) &&
                    r.ReservedAt > now.AddMinutes(-5));

                if (reservedSeats.Any())
                {
                    return new ConfirmCheckoutResultDto { Success = false, Message = "Some selected seats are already reserved." };
                }

                // Temporarily reserve the seats
                foreach (var seatId in model.SelectedSeatIds)
                {
                    var reservation = new TemporarySeatReservation
                    {
                        ShowTimeId = model.ShowTimeId,
                        SeatId = seatId,
                        UserId = user.Id,
                        ReservedAt = now
                    };

                    _unitOfWork.TemporarySeatReservations.Add(reservation);
                }

                // Save changes
                _unitOfWork.Save();

                // Return success response
                return new ConfirmCheckoutResultDto { Success = true, Message = "Seats reserved successfully." };
            }
            catch (Exception ex)
            {
                // Log the exception if required
                Console.WriteLine($"Error: {ex.Message}");
                return new ConfirmCheckoutResultDto { Success = false, Message = "An error occurred while reserving seats. Please try again." };
            }




        }



        public async Task<CheckoutConfirmationDto> CheckoutConfirmationAsync(int showTimeId, string selectedSeats, string userName)
        {
            // Fetch authenticated user
            if (string.IsNullOrEmpty(userName))
            {
                return null; // Unauthorized scenario
            }

            var user = await _userManager.FindByNameAsync(userName);
            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();

            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            if (showTime == null)
            {
                return null; // ShowTime not found
            }

            var movie = _unitOfWork.Movies.Get(x => x.MovieId == showTime.MovieId);
            var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == showTime.TheatreId);
            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);

            var seatNumbers = _unitOfWork.Seats.GetAll(x => seatIds.Contains(x.SeatId))
                .Select(s => $"{s.SectionName} {s.SeatNumber}")
                .ToList();

            var totalPrice = showTime.Price * seatIds.Count;

            // Fetch the earliest reservation time for the selected seats
            var reservedAt = _unitOfWork.TemporarySeatReservations
                .GetAll(r => r.UserId == user.Id && seatIds.Contains(r.SeatId) && r.ShowTimeId == showTimeId)
                .OrderBy(r => r.ReservedAt)
                .Select(r => r.ReservedAt)
                .FirstOrDefault();

            if (reservedAt == default)
            {
                return null; // Reservation not found or expired
            }

            return new CheckoutConfirmationDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserEmail = user.Email,
                ShowTimeId = showTimeId,
                MovieTitle = movie.Title,
                PosterUrl = movie.Poster,
                TheatreName = theatre.TheatreName,
                Genre = movie.Genre,
                Duration = movie.Duration,
                HallName = hall.HallName,
                ShowDate = showTime.ShowDate.ToString("MMMM dd, yyyy"),
                ShowTime = $"{showTime.ShowTimeStart} - {showTime.ShowTimeEnd}",
                SelectedSeatNumbers = seatNumbers,
                TotalPrice = totalPrice,
                ReservedAt = reservedAt,
                SelectedSeatIds = seatIds
            };
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

        public async Task<FinalizeBookingResultDto> FinalizeBookingAsync(FinalizeBookingDto model, string domain)
        {
            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == model.ShowTimeId, includeProperties: "Movie");
            if (showTime == null || showTime.Movie == null)
            {
                return new FinalizeBookingResultDto { Success = false, Message = "ShowTime or Movie data is missing." };
            }

            var totalPrice = showTime.Price * model.SelectedSeatIds.Count;
            if (totalPrice <= 0)
            {
                return new FinalizeBookingResultDto { Success = false, Message = "Invalid total price calculation." };
            }

            if (model.SelectedSeatIds == null || !model.SelectedSeatIds.Any())
            {
                return new FinalizeBookingResultDto { Success = false, Message = "SelectedSeatIds is null or empty." };
            }

            if (string.IsNullOrEmpty(domain))
            {
                return new FinalizeBookingResultDto { Success = false, Message = "Domain is not set." };
            }

            var stripeUrl = await _stripeService.CreateStripeSessionAsync(model.ShowTimeId, model.SelectedSeatIds, totalPrice, domain);

            return new FinalizeBookingResultDto
            {
                Success = true,
                RedirectUrl = stripeUrl,
                Message = "Redirecting to payment."
            };
        }

        public async Task<PaymentResultDto> ProcessPaymentSuccessAsync(string sessionId, string userName)
        {
            return await _stripeService.ProcessPaymentSuccessAsync(sessionId, userName);
        }

        public async Task<PaymentResultDto> ProcessPaymentFailedAsync(string sessionId, string userName)
        {
            return await _stripeService.ProcessPaymentFailedAsync(sessionId, userName);
        }

        public async Task<List<TicketDto>> GetSuccessfulReservationsAsync(string reservationIds)
        {
            try
            {
                var ids = reservationIds.Split(',').Select(int.Parse).ToList();
                // Fetch reservations by IDs
                var reservations = _unitOfWork.Reservations.GetAll(r => ids.Contains(r.ReservationId),
                    includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat")
                    .Select(r => new TicketDto
                    {
                        TicketId = r.ReservationId,// Include Reservation ID
                        MovieTitle = r.ShowTime.Movie.Title,
                        TheatreName = r.ShowTime.Theatre.TheatreName,
                        HallName = _unitOfWork.Halls.Get(x => x.HallId == r.ShowTime.HallId).HallName,
                        SeatNumber = $"{r.Seat.SectionName} {r.Seat.SeatNumber}",
                        ShowDate = r.ShowTime.ShowDate.ToString("MMMM dd, yyyy"),
                        ShowTime = $"{r.ShowTime.ShowTimeStart} - {r.ShowTime.ShowTimeEnd}",
                        Price = r.ShowTime.Price
                    }).ToList();

                return reservations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSuccessfulReservationsAsync: {ex.Message}");
                return new List<TicketDto>(); // Return empty list if error occurs
            }
        }
    }
}
