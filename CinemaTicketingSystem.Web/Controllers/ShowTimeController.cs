using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class ShowTimeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowTimeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult ShowTimeManagement()
        {
            var showTimes = _unitOfWork.ShowTimes.GetAll(includeProperties: "Theatre,Movie").ToList();
            var showTimeCount = showTimes.Count;
            var theatreCount = showTimes.Select(s => s.TheatreId).Distinct().Count();
            var movieCount = showTimes.Select(s => s.MovieId).Distinct().Count();
            var totalMinutes = showTimes.Sum(s => (s.ShowTimeEnd - s.ShowTimeStart).TotalMinutes);

            var model = new ShowTimeManagementVM
            {
                ShowTimes = showTimes,
                TotalShowTimes = showTimeCount,
                TotalTheatres = theatreCount,
                TotalMovies = movieCount,
                TotalMinutes = totalMinutes
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult AddShowTime()
        {
            var model = new AddShowTimeVM
            {
                Theatres = _unitOfWork.Theatres.GetAll().ToList(),
                Movies = _unitOfWork.Movies.GetAll().ToList(),
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetHallsByTheatre(int theatreId)
        {
            var halls = _unitOfWork.Halls
                .GetAll(h => h.TheatreId == theatreId)
                .Select(h => new { h.HallId, h.HallName })
                .ToList();

            return Json(halls);
        }


        [HttpPost]
        public IActionResult AddShowTime(AddShowTimeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Theatres = _unitOfWork.Theatres.GetAll().ToList();
                model.Movies = _unitOfWork.Movies.GetAll().ToList();
                return View(model);
            }

            var showTime = new ShowTime
            {
                ShowDate = model.ShowDate,
                ShowTimeStart = model.ShowTimeStart,
                ShowTimeEnd = model.ShowTimeEnd,
                TheatreId = model.TheatreId,
                MovieId = model.MovieId,
                Price = model.Price,
                HallId = model.HallId,
            };

            _unitOfWork.ShowTimes.Add(showTime);
            _unitOfWork.Save();

            TempData["success"] = "Show Time added successfully!";
            return RedirectToAction("ShowTimeManagement");
        }


        [HttpGet]
        public IActionResult SearchShowTimes(string query, string filterBy)
        {
            // Load all ShowTimes with related properties
            var showTimes = _unitOfWork.ShowTimes.GetAll(includeProperties: "Theatre,Movie").ToList();

            // Apply search filter if query is provided
            if (!string.IsNullOrWhiteSpace(query))
            {
                switch (filterBy?.ToLower())
                {
                    case "movie":
                        showTimes = showTimes.Where(s => s.Movie.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "theatre":
                        showTimes = showTimes.Where(s => s.Theatre.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default:
                        // Optionally handle invalid filterBy
                        return BadRequest(new { message = "Invalid filterBy value. Use 'movie' or 'theatre'." });
                }
            }

            // Load all Halls for mapping
            var halls = _unitOfWork.Halls.GetAll().ToList();

            // Project the result
            var result = showTimes.Select(s => new
            {
                s.ShowTimeId,
                ShowDate = s.ShowDate.ToShortDateString(),
                StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
                EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
                Theatre = s.Theatre?.TheatreName ?? "N/A",
                Hall = halls.FirstOrDefault(h => h.HallId == s.HallId)?.HallName ?? "N/A",
                Movie = s.Movie?.Title ?? "N/A",
                Price = s.Price
            }).ToList();

            return Json(result);
        }


        [HttpGet]
        public IActionResult EditShowTime(int id)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == id);
            if (showTime == null) return NotFound();

            var model = new EditShowTimeVM
            {
                ShowTimeId = showTime.ShowTimeId,
                ShowDate = showTime.ShowDate,
                ShowTimeStart = showTime.ShowTimeStart,
                ShowTimeEnd = showTime.ShowTimeEnd,
                TheatreId = showTime.TheatreId,
                MovieId = showTime.MovieId,
                Price = showTime.Price,
                Theatres = _unitOfWork.Theatres.GetAll().ToList(),
                Movies = _unitOfWork.Movies.GetAll().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditShowTime(EditShowTimeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Theatres = _unitOfWork.Theatres.GetAll().ToList();
                model.Movies = _unitOfWork.Movies.GetAll().ToList();
                return View(model);
            }

            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == model.ShowTimeId);
            if (showTime == null) return NotFound();

            showTime.ShowDate = model.ShowDate;
            showTime.ShowTimeStart = model.ShowTimeStart;
            showTime.ShowTimeEnd = model.ShowTimeEnd;
            showTime.TheatreId = model.TheatreId;
            showTime.MovieId = model.MovieId;
            showTime.Price = model.Price;

            _unitOfWork.ShowTimes.Update(showTime);
            _unitOfWork.Save();

            TempData["success"] = "Show Time updated successfully!";
            return RedirectToAction("ShowTimeManagement");
        }

        [HttpPost]
        public IActionResult DeleteShowTime(int id)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == id);
            if (showTime == null) return NotFound();

            _unitOfWork.ShowTimes.Remove(showTime);
            _unitOfWork.Save();

            TempData["success"] = "Show Time deleted successfully!";
            return RedirectToAction("ShowTimeManagement");
        }

    }
}
