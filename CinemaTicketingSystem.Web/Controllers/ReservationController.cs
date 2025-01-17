using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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


    }
}
