using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;

        public ReservationController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IReservationService reservationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _reservationService = reservationService;
        }

        [HttpGet]
        public IActionResult ManageReservation() => View();

        public async Task<IActionResult> SearchReservationByShowTime(string query, string filterBy)
        {
            //// Validate input
            //if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
            //{
            //    return BadRequest(new { message = "Query and filterBy are required." });
            //}

            //List<int> relevantIds = new List<int>();

            //// Filter based on the criteria
            //switch (filterBy.ToLower())
            //{
            //    case "movie":
            //        // Fetch relevant MovieIds
            //        relevantIds = _unitOfWork.Movies
            //            .GetAll()
            //            .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            //            .Select(m => m.MovieId)
            //            .ToList();
            //        break;

            //    case "theatre":
            //        // Fetch relevant TheatreIds
            //        relevantIds = _unitOfWork.Theatres
            //            .GetAll()
            //            .Where(t => t.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase))
            //            .Select(t => t.TheatreId)
            //            .ToList();
            //        break;

            //    default:
            //        return BadRequest(new { message = "Invalid filterBy value. Use 'movie' or 'theatre'." });
            //}

            //// Fetch ShowTimes based on the filtered IDs
            //var showTimes = filterBy.ToLower() == "movie"
            //    ? _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.MovieId))
            //    : _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.TheatreId));


            //// Project the result
            //var result = showTimes.Select(s => new
            //{
            //    s.ShowTimeId,
            //    ShowDate = s.ShowDate.ToShortDateString(),
            //    StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
            //    EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
            //    Theatre = _unitOfWork.Theatres.GetAll().FirstOrDefault(t => t.TheatreId == s.TheatreId)?.TheatreName ?? "N/A",
            //    Hall = _unitOfWork.Halls.GetAll().FirstOrDefault(h => h.HallId == s.HallId)?.HallName ?? "N/A",
            //    Movie = _unitOfWork.Movies.GetAll().FirstOrDefault(t => t.MovieId == s.MovieId)?.Title ?? "N/A",
            //    Price = s.Price,
            //    TotalSeats = _unitOfWork.Seats.GetAll().Count(seat => seat.HallId == s.HallId),
            //    ReservedSeats = _unitOfWork.Reservations.GetAll().Count(r => r.ShowTimeId == s.ShowTimeId && r.PaymentStatus == SD.PaymentStatus_Paid)
            //}).ToList();


            //return Json(result);


            var result = await _reservationService.SearchReservationByShowTimeAsync(query, filterBy);
            return Json(result);


        }

        public async Task<IActionResult> SearchReservationByUserOrTicket(string query, string filterBy)
        {
            //// Validate input
            //if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
            //{
            //    return BadRequest(new { message = "Query and filterBy are required." });
            //}

            //ApplicationUser user = null;
            //int ticketId = 0;

            //switch (filterBy.ToLower())
            //{
            //    case "user":
            //        // Fetch reservations by user
            //        user = _userManager.Users.FirstOrDefault(u => u.Email == query.ToLower());
            //        if (user == null)
            //        {
            //            return NotFound(new { message = "User not found." });
            //        }
            //        break;

            //    case "ticket":

            //        if (query.Contains("@"))
            //        {
            //            return BadRequest(new { message = "For ticket search, please enter the ticket number, not an email address." });
            //        }

            //        // Attempt to parse the input into an integer
            //        if (!int.TryParse(query, out ticketId))
            //        {
            //            return BadRequest(new { message = "The ticket input must be a valid number." });
            //        }

            //        // Search for the reservation by ticket ID
            //        var reservation = _unitOfWork.Reservations.Get(r => r.ReservationId == ticketId);
            //        if (reservation == null)
            //        {
            //            return NotFound(new { message = "Ticket not found." });
            //        }

            //        // Retrieve the user associated with the reservation
            //        user = _userManager.Users.FirstOrDefault(u => u.Id == reservation.UserId);

            //        break;

            //    default:
            //        return BadRequest(new { message = "Invalid query value. Use 'user' or 'ticket'." });
            //}

            //// Fetch reservations based on the user or ticket ID
            //var reservations = filterBy.ToLower() == "user"
            //    ? _unitOfWork.Reservations.GetAll(r => r.UserId == user.Id, 
            //    includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat")
            //    : _unitOfWork.Reservations.GetAll(r => r.ReservationId == ticketId,
            //    includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat");

            //// Project the result
            //var result = reservations
            //    .OrderByDescending(r => r.ReservationId)
            //    .Select(r => new
            //{
            //    TicketNumber = r.ReservationId,
            //    UserEmail = user.Email,
            //    Movie = r.ShowTime.Movie.Title,
            //    Theatre = r.ShowTime.Theatre.TheatreName,
            //    Hall = _unitOfWork.Halls.Get(h => h.HallId == r.ShowTime.HallId).HallName,
            //    ShowDate = r.ShowTime.ShowDate.ToShortDateString(),
            //    SeatNumber = $"{r.Seat.SectionName} {r.Seat.SeatNumber}",
            //    Status = r.Status,
            //    PaymentStatus = r.PaymentStatus                
            //}).ToList();

            //return Json(result);

            var result = await _reservationService.SearchReservationByUserOrTicketAsync(query, filterBy);
            return Json(result);
        }



        [HttpGet]
        public IActionResult CreateReservation(int? movieId, int? theatreId)
        {
            var vm = new ReservationViewModel
            {
                MovieId = movieId,
                TheatreId = theatreId,
            };
            ViewData["HeroImageUrl"] = "/images/hero-banner.jpg";
            ViewData["HeroTitle"] = "Lights, Camera, Action!";
            ViewData["HeroSubtitle"] = "Catch the latest blockbusters and reserve your seats now.";
            return View(vm);
        }

        [HttpGet]
        public IActionResult GetMoviesAndTheatres()
        {
            //var movies = _unitOfWork.Movies.GetAll()
            //    .Select(m => new { movieId = m.MovieId, title = m.Title }).ToList();
            //var theatres = _unitOfWork.Theatres.GetAll()
            //    .Select(t => new { theatreId = t.TheatreId, name = t.TheatreName }).ToList();

            var data = _reservationService.GetMoviesAndTheatresAsync();

            var result = new
            {
                movies = data.Result.Movies,
                theatres = data.Result.Theatres
            };

            return Json(new { success = true, result.movies, result.theatres });
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredShowTimes([FromBody] ReservationViewModel filter)
        {
            //var query = _unitOfWork.ShowTimes.GetAll(includeProperties: "Movie,Theatre")
            //    .Where(st => st.ShowDate == model.ShowDate);

            //if (model.MovieId.HasValue)
            //    query = query.Where(st => st.MovieId == model.MovieId.Value);

            //if (model.TheatreId.HasValue)
            //    query = query.Where(st => st.TheatreId == model.TheatreId.Value);

            //var showTimes = query.Select(st => new
            //{
            //    showTimeId = st.ShowTimeId,
            //    movieTitle = st.Movie.Title,
            //    theatreName = st.Theatre.TheatreName,
            //    startTime = st.ShowTimeStart.ToString(@"hh\:mm"),
            //    endTime = st.ShowTimeEnd.ToString(@"hh\:mm"),
            //    price = st.Price
            //}).ToList();

            var ReservationFilterDto = new ReservationFilterDto
            {
                ShowDate = filter.ShowDate,
                MovieId = filter.MovieId,
                TheatreId = filter.TheatreId
            };

            var showTimes = await _reservationService.GetFilteredShowTimesAsync(ReservationFilterDto);

            return Json(showTimes);
        }


        public IActionResult BookShowTime(int showTimeId)
        {
            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            var movie = _unitOfWork.Movies.Get(x => x.MovieId == showTime.MovieId);
            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);
            var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == showTime.TheatreId);

            BookShowTimeVM model = new BookShowTimeVM
            {
                ShowTime = showTime,
                Movie = movie,
                Hall = hall,
                Theatre = theatre
            };

            return View(model);
        }

        public IActionResult ProceedBookingSeat(int showTimeId, int seatCount)
        {

            // Fetch the ShowTime entity
            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            if (showTime == null)
            {
                return NotFound("ShowTime not found.");
            }

            // Fetch the Hall associated with the ShowTime
            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);
            if (hall == null)
            {
                return NotFound("Hall not found.");
            }

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
                .Select(s => new SeatVM
                {
                    SeatId = s.SeatId,
                    SectionName = s.SectionName,
                    SeatNumber = s.SeatNumber,
                    IsReserved = confirmedReservations.Contains(s.SeatId), // Check against confirmed reservations
                    IsTemporaryReserved = temporaryReservations.Any(r => r.SeatId == s.SeatId) // Check against temporary reservations
                }).ToList();

            // Populate the view model
            var model = new ProceedBookingSeatVM
            {
                ShowTimeId = showTimeId,
                SeatCount = seatCount,
                HallName = hall.HallName,
                Seats = seats,
                Sections = sectionInfo.Select(s => new SectionVM
                {
                    SectionName = s.SectionName,
                    SectionCount = s.SectionCount
                }).ToList()
            };

            // Return the view with the model
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout([FromBody] ConfirmCheckoutVM model)
        {
            try
            {
                // Validate input
                if (model == null || model.SelectedSeatIds == null || !model.SelectedSeatIds.Any())
                {
                    return BadRequest(new { message = "Invalid input. Please select at least one seat." });
                }

                // Get the current user from Identity
                var userName = User.Identity.Name; // Assumes Identity uses Username as User.Identity.Name
                var user = await _userManager.FindByNameAsync(userName);
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new { message = "User not authenticated." });
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
                    return Conflict(new { message = "Some selected seats are already reserved." });
                }

                // Temporarily reserve the seats
                foreach (var seatId in model.SelectedSeatIds)
                {
                    var reservation = new TemporarySeatReservation
                    {
                        ShowTimeId = model.ShowTimeId,
                        SeatId = seatId,
                        UserId = user.Id,
                        ReservedAt = now,
                        //IsConfirmed = false
                    };

                    _unitOfWork.TemporarySeatReservations.Add(reservation);
                }

                // Save changes
                _unitOfWork.Save();

                // Return success response
                return Ok(new { success = true, message = "Seats reserved successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception if required
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while reserving seats. Please try again." });
            }
        }

        public async Task<IActionResult> CheckoutConfirmation(int showTimeId, string selectedSeats)
        {

            var userName = User.Identity.Name; // Fetch authenticated user
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User not authenticated.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();

            var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == showTimeId);
            if (showTime == null)
            {
                return NotFound("ShowTime not found.");
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
                return BadRequest("Reservation not found or expired.");
            }

            var model = new CheckoutConfirmationVM
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
                ReservedAt = reservedAt, // Pass the reservation time to the view
                SelectedSeatIds = seatIds,
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> FinalizeBooking([FromBody] FinalizeBookingRequestVM model)
        {
            try
            {
                var showTime = _unitOfWork.ShowTimes.Get(x => x.ShowTimeId == model.ShowTimeId, includeProperties: "Movie");
                if (showTime == null || showTime.Movie == null)
                {
                    throw new Exception("ShowTime or Movie data is missing.");
                }

                var totalPrice = showTime.Price * model.SelectedSeatIds.Count;
                if (totalPrice <= 0)
                {
                    throw new Exception("Invalid total price calculation.");
                }

                if (model.SelectedSeatIds == null || !model.SelectedSeatIds.Any())
                {
                    throw new Exception("SelectedSeatIds is null or empty.");
                }

                var domain = $"{Request.Scheme}://{Request.Host}";
                if (string.IsNullOrEmpty(domain))
                {
                    throw new Exception("Domain is not set.");
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "cad", // Set currency to Canadian Dollar
                                UnitAmount = (long)(totalPrice * 100), // Amount in cents
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"{showTime.Movie.Title} - Reservation",
                                    Description = $"Seats: {string.Join(", ", model.SelectedSeatIds)}"
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"{domain}/Reservation/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{domain}/Reservation/PaymentFailed?sessionId={{CHECKOUT_SESSION_ID}}",

                    Metadata = new Dictionary<string, string>
                    {
                        { "showTimeId", model.ShowTimeId.ToString() },
                        { "selectedSeatIds", string.Join(",", model.SelectedSeatIds) }
                    }

                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                // Redirect to Stripe payment page
                return Json(new { success = true, redirectUrl = session.Url });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error in FinalizeBooking: {ex.Message}");
                return StatusCode(500, "An error occurred while finalizing the booking.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(string sessionId)
        {
            try
            {
                // Initialize Stripe's session service
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                // Verify payment status
                if (session.PaymentStatus == "paid")
                {
                    // Retrieve metadata from the Stripe session
                    var showTimeId = int.Parse(session.Metadata["showTimeId"]);
                    var selectedSeatIds = session.Metadata["selectedSeatIds"]
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();

                    // Get the current user ID from Identity or session data
                    var userName = User.Identity?.Name;
                    var user = await _userManager.FindByNameAsync(userName);
                    var userId = user?.Id ?? session.CustomerEmail; // Fallback to customer email if user not found

                    // Create a list to store reservation IDs
                    var reservationIds = new List<int>();


                    // Add entries to the Reservation table
                    foreach (var seatId in selectedSeatIds)
                    {
                        var reservation = new Reservation
                        {
                            ShowTimeId = showTimeId,
                            SeatId = seatId,
                            ReservationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = "Confirmed", // Reservation status
                            PaymentStatus = "Paid", // Payment status
                            UserId = userId
                        };

                        _unitOfWork.Reservations.Add(reservation);
                        _unitOfWork.Save();

                        reservationIds.Add(reservation.ReservationId); // Collect the reservation ID


                        // Add an entry to the Payment table
                        var totalPrice = session.AmountTotal / 100.0; // Convert cents to dollars
                        var payment = new Payment
                        {
                            ReservationId = reservation.ReservationId,
                            Amount = (decimal)totalPrice,
                            PaymentDate = reservation.ReservationDate,
                            PaymentStatus = "Paid",
                            StripeSessionId = sessionId,
                            StripePaymentIntentId = session.PaymentIntentId
                        };

                        _unitOfWork.Payments.Add(payment);
                        _unitOfWork.Save();
                    }

                    // Redirect to the success page with reservation IDs
                    return RedirectToAction("SuccessPage", new { reservationIds = string.Join(",", reservationIds) });
                }
                else
                {
                    // Redirect to a failure page if payment status is not paid
                    return RedirectToAction("PaymentFailed");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in PaymentSuccess: {ex.Message}");
                return RedirectToAction("PaymentFailed");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentFailed(string sessionId)
        {
            try
            {
                // Initialize Stripe's session service
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                // Retrieve metadata from the Stripe session
                var showTimeId = int.Parse(session.Metadata["showTimeId"]);
                var selectedSeatIds = session.Metadata["selectedSeatIds"]
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                var totalPrice = decimal.Parse(session.AmountTotal.ToString()) / 100; // Convert from cents to dollars

                // Get the current user ID from Identity or session data
                var userName = User.Identity?.Name;
                var user = await _userManager.FindByNameAsync(userName);
                var userId = user?.Id ?? session.CustomerEmail; // Fallback to customer email if user not found

                // Record each reservation with "Failed" status
                foreach (var seatId in selectedSeatIds)
                {
                    // Add entry to the Reservation table
                    var reservation = new Reservation
                    {
                        ShowTimeId = showTimeId,
                        SeatId = seatId,
                        ReservationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Status = "Failed",
                        PaymentStatus = "Failed",
                        UserId = userId
                    };
                    _unitOfWork.Reservations.Add(reservation);

                    // Save all changes
                    _unitOfWork.Save();

                    // Add failed payment data to the Payment table
                    var payment = new Payment
                    {
                        ReservationId = reservation.ReservationId,
                        Amount = totalPrice,
                        PaymentDate = reservation.ReservationDate,
                        PaymentStatus = "Failed",
                        StripeSessionId = sessionId,
                        StripePaymentIntentId = session.PaymentIntentId
                    };
                    _unitOfWork.Payments.Add(payment);

                    // Save all changes
                    _unitOfWork.Save();
                }

                // Rollback temporary reservations
                foreach (var seatId in selectedSeatIds)
                {
                    var tempReservation = _unitOfWork.TemporarySeatReservations
                        .Get(r => r.ShowTimeId == showTimeId && r.SeatId == seatId);

                    if (tempReservation != null)
                    {
                        // Remove the temporary reservation
                        _unitOfWork.TemporarySeatReservations.Remove(tempReservation);

                        // Save all changes
                        _unitOfWork.Save();
                    }
                }

                // Notify the user
                TempData["error"] = "Payment failed. Your selected seats have been released. Please try again.";

                // Redirect to the seat selection page for the same showtime
                return RedirectToAction("ProceedBookingSeat", new { showTimeId, seatCount = selectedSeatIds.Count });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in PaymentFailed: {ex.Message}");

                // Notify the user of a generic error
                TempData["error"] = "An unexpected error occurred. Please try again.";

                // Redirect to the home page or another fallback page
                return RedirectToAction("FailedPaymentPage", "Reservation");
            }
        }

        [HttpGet]
        public IActionResult SuccessPage(string reservationIds)
        {
            try
            {
                var ids = reservationIds.Split(',').Select(int.Parse).ToList();

                // Fetch reservations by IDs
                var reservations = _unitOfWork.Reservations.GetAll(r => ids.Contains(r.ReservationId),
                    includeProperties: "ShowTime.Movie,ShowTime.Theatre,Seat")
                    .Select(r => new TicketVM
                    {
                        TicketId = r.ReservationId, // Include Reservation ID
                        MovieTitle = r.ShowTime.Movie.Title,
                        TheatreName = r.ShowTime.Theatre.TheatreName,
                        HallName = _unitOfWork.Halls.Get(x => x.HallId == r.ShowTime.HallId).HallName,
                        SeatNumber = $"{r.Seat.SectionName} {r.Seat.SeatNumber}",
                        ShowDate = r.ShowTime.ShowDate.ToString("MMMM dd, yyyy"),
                        ShowTime = $"{r.ShowTime.ShowTimeStart} - {r.ShowTime.ShowTimeEnd}",
                        Price = r.ShowTime.Price
                    }).ToList();

                if (!reservations.Any())
                {
                    return NotFound("No tickets found.");
                }

                return View(reservations); // Pass the tickets to the view
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SuccessPage: {ex.Message}");
                return RedirectToAction("ErrorPage", new { message = "Error fetching tickets." });
            }
        }

        [HttpGet]
        public IActionResult FailedPaymentPage()
        {
            return View();
        }




    }
}
