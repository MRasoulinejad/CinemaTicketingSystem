using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class ShowTimeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IShowTimeService _showTimeService;

        public ShowTimeController(IUnitOfWork unitOfWork, IShowTimeService showTimeService, ITheatreService theatreService, IMovieService movieService)
        {
            _unitOfWork = unitOfWork;
            _showTimeService = showTimeService;
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> ShowTimeManagement()
        {
            //var showTimes = _unitOfWork.ShowTimes.GetAll(includeProperties: "Theatre,Movie").ToList();
            //var showTimeCount = showTimes.Count();
            //var theatreCount = showTimes.Select(s => s.TheatreId).Distinct().Count();
            //var movieCount = showTimes.Select(s => s.MovieId).Distinct().Count();
            //var totalMinutes = showTimes.Sum(s => (s.ShowTimeEnd - s.ShowTimeStart).TotalMinutes);

            //var model = new ShowTimeManagementVM
            //{
            //    ShowTimes = showTimes,
            //    TotalShowTimes = showTimeCount,
            //    TotalTheatres = theatreCount,
            //    TotalMovies = movieCount,
            //    TotalMinutes = totalMinutes
            //};

            //return View(model);

            //var model = await _showTimeService.GetAllShowTimesAsync();

            //var viewModel = new ShowTimeManagementVM
            //{
            //    ShowTimes = model.ShowTimes,
            //    TotalShowTimes = model.TotalShowTimes,
            //    TotalTheatres = model.TotalTheatres,
            //    TotalMovies = model.TotalMovies,
            //    TotalMinutes = model.TotalMinutes
            //};

            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> AddShowTime()
        {
            var data = await _showTimeService.GetAddShowTimeDataAsync();

            var model = new AddShowTimeVM
            {
                Theatres = data.Theatres,
                Movies = data.Movies
            };
            return View(model);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> GetHallsByTheatre(int theatreId)
        { 
            //var halls = _unitOfWork.Halls
            //    .GetAll(h => h.TheatreId == theatreId)
            //    .Select(h => new { h.HallId, h.HallName })
            //    .ToList();


            var halls = await _showTimeService.GetHallsByTheatreAsync(theatreId);

            return Json(halls);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> AddShowTime(AddShowTimeVM model)
        {
            if (!ModelState.IsValid)
            {
                var data = await _showTimeService.GetAddShowTimeDataAsync();

                model.Theatres = data.Theatres;
                model.Movies = data.Movies;
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

            await _showTimeService.AddShowTimeAsync(showTime);

            TempData["success"] = "Show Time added successfully!";
            return RedirectToAction("ShowTimeManagement");
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> SearchShowTimes(string query, string filterBy)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
            {
                return BadRequest(new { message = "Query and filterBy are required." });
            }

            var result = await _showTimeService.SearchShowTimesAsync(query, filterBy);


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
            //    Price = s.Price
            //}).ToList();

            return Json(result);


            //// Load all ShowTimes with related properties
            //var showTimes = _unitOfWork.ShowTimes.GetAll(includeProperties: "Theatre,Movie").ToList();

            //// Apply search filter if query is provided
            //if (!string.IsNullOrWhiteSpace(query))
            //{
            //    switch (filterBy?.ToLower())
            //    {
            //        case "movie":
            //            showTimes = showTimes.Where(s => s.Movie.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            //            break;
            //        case "theatre":
            //            showTimes = showTimes.Where(s => s.Theatre.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            //            break;
            //        default:
            //            // Optionally handle invalid filterBy
            //            return BadRequest(new { message = "Invalid filterBy value. Use 'movie' or 'theatre'." });
            //    }
            //}

            //// Load all Halls for mapping
            //var halls = _unitOfWork.Halls.GetAll().ToList();

            //// Project the result
            //var result = showTimes.Select(s => new
            //{
            //    s.ShowTimeId,
            //    ShowDate = s.ShowDate.ToShortDateString(),
            //    StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
            //    EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
            //    Theatre = s.Theatre?.TheatreName ?? "N/A",
            //    Hall = halls.FirstOrDefault(h => h.HallId == s.HallId)?.HallName ?? "N/A",
            //    Movie = s.Movie?.Title ?? "N/A",
            //    Price = s.Price
            //}).ToList();

            //return Json(result);
        }


        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> EditShowTime(int showTimeId)
        {
            var showTime = await _showTimeService.GetEditShowTimeDataAsync(showTimeId);

            //var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == showTimeId);
            //if (showTime == null) return NotFound();

            //var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);

            var model = new EditShowTimeVM
            {
                ShowTimeId = showTime.ShowTimeId,
                ShowDate = showTime.ShowDate,
                ShowTimeStart = showTime.ShowTimeStart,
                ShowTimeEnd = showTime.ShowTimeEnd,
                TheatreId = showTime.TheatreId,
                MovieId = showTime.MovieId,
                Price = showTime.Price,
                Theatres = showTime.Theatres,
                Movies = showTime.Movies,
                HallId = showTime.HallId,
                HallName = showTime.HallName
            };

            return View(model);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> EditShowTime(EditShowTimeVM model)
        {
            if (!ModelState.IsValid)
            {
                //model.Theatres = _unitOfWork.Theatres.GetAll().ToList();
                //model.Movies = _unitOfWork.Movies.GetAll().ToList();
                //return View(model);

                var data = await _showTimeService.GetAddShowTimeDataAsync();

                model.Theatres = data.Theatres;
                model.Movies = data.Movies;
                return View(model);
            }

            //var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == model.ShowTimeId);
            //if (showTime == null) return NotFound();

            //showTime.ShowDate = model.ShowDate;
            //showTime.ShowTimeStart = model.ShowTimeStart;
            //showTime.ShowTimeEnd = model.ShowTimeEnd;
            //showTime.TheatreId = model.TheatreId;
            //showTime.HallId = model.HallId;
            //showTime.MovieId = model.MovieId;
            //showTime.Price = model.Price;

            //_unitOfWork.ShowTimes.Update(showTime);
            //_unitOfWork.Save();


            try
            {
                await _showTimeService.UpdateShowTimeAsync(new UpdateShowTimeDto
                {
                    ShowTimeId = model.ShowTimeId,
                    ShowDate = model.ShowDate,
                    ShowTimeStart = model.ShowTimeStart,
                    ShowTimeEnd = model.ShowTimeEnd,
                    TheatreId = model.TheatreId,
                    HallId = model.HallId,
                    MovieId = model.MovieId,
                    Price = model.Price
                });

                TempData["success"] = "Show Time updated successfully!";
                return RedirectToAction("ShowTimeManagement");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("ShowTimeManagement");
            }
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShowTime([FromBody] int showTimeId)
        {
            //var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == showTimeId);

            //if (showTime != null)
            //{
            //    try
            //    {
            //        _unitOfWork.ShowTimes.Remove(showTime);
            //        _unitOfWork.Save();
            //        return Json(new { success = true });
            //    }
            //    catch (Exception ex)
            //    {
            //        return Json(new { success = false, message = ex.Message });
            //    }
            //}
            //return Json(new { success = false, message = "Show Time is Null!" });

            var showTime = await _showTimeService.DeleteShowTimeAsync(showTimeId);

            if (showTime)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Show Time is Null!" });
            }
        }
    }
}