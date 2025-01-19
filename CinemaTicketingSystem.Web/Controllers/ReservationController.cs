using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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
            var movies = _unitOfWork.Movies.GetAll()
                .Select(m => new { movieId = m.MovieId, title = m.Title }).ToList();
            var theatres = _unitOfWork.Theatres.GetAll()
                .Select(t => new { theatreId = t.TheatreId, name = t.TheatreName }).ToList();

            return Json(new { movies, theatres });
        }

        [HttpPost]
        public IActionResult GetFilteredShowTimes([FromBody] ReservationViewModel model)
        {
            var query = _unitOfWork.ShowTimes.GetAll(includeProperties: "Movie,Theatre")
                .Where(st => st.ShowDate == model.ShowDate);

            if (model.MovieId.HasValue)
                query = query.Where(st => st.MovieId == model.MovieId.Value);

            if (model.TheatreId.HasValue)
                query = query.Where(st => st.TheatreId == model.TheatreId.Value);

            var showTimes = query.Select(st => new
            {
                showTimeId = st.ShowTimeId,
                movieTitle = st.Movie.Title,
                theatreName = st.Theatre.TheatreName,
                startTime = st.ShowTimeStart.ToString(@"hh\:mm"),
                endTime = st.ShowTimeEnd.ToString(@"hh\:mm"),
                price = st.Price
            }).ToList();

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

            var now = DateTime.Now;

            // Fetch all relevant temporary reservations for the given ShowTime within the last 5 minutes
            var temporaryReservations = _unitOfWork.TemporarySeatReservations.GetAll(r =>
                r.ShowTimeId == showTimeId && r.ReservedAt > now.AddMinutes(-5)).ToList();

            // Fetch and group seats by sections
            var sectionInfo = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
                .GroupBy(s => s.SectionName)
                .Select(group => new
                {
                    SectionName = group.Key,
                    SectionCount = group.Count()
                }).ToList();

            // Fetch seat data, including temporary reservation status
            var seats = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
                .Select(s => new SeatVM
                {
                    SeatId = s.SeatId,
                    SectionName = s.SectionName,
                    SeatNumber = s.SeatNumber,
                    IsReserved = s.IsReserved,
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




    }
}
