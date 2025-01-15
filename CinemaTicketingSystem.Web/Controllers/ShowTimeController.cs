﻿using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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
        public async Task<IActionResult> SearchShowTimes(string query, string filterBy)
        {

            // Validate input
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(filterBy))
            {
                return BadRequest(new { message = "Query and filterBy are required." });
            }

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
                    return BadRequest(new { message = "Invalid filterBy value. Use 'movie' or 'theatre'." });
            }

            // Fetch ShowTimes based on the filtered IDs
            var showTimes = filterBy.ToLower() == "movie"
                ? _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.MovieId))
                : _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.TheatreId));


            // Project the result
            var result = showTimes.Select(s => new
            {
                s.ShowTimeId,
                ShowDate = s.ShowDate.ToShortDateString(),
                StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
                EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
                Theatre = _unitOfWork.Theatres.GetAll().FirstOrDefault(t => t.TheatreId == s.TheatreId)?.TheatreName ?? "N/A",
                Hall = _unitOfWork.Halls.GetAll().FirstOrDefault(h => h.HallId == s.HallId)?.HallName ?? "N/A",
                Movie = _unitOfWork.Movies.GetAll().FirstOrDefault(t => t.MovieId == s.MovieId)?.Title ?? "N/A",
                Price = s.Price
            }).ToList();

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



        [HttpGet]
        public IActionResult EditShowTime(int showTimeId)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == showTimeId);
            if (showTime == null) return NotFound();

            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);

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
                Movies = _unitOfWork.Movies.GetAll().ToList(),
                HallId = hall.HallId,
                HallName = hall.HallName
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
            showTime.HallId = model.HallId;
            showTime.MovieId = model.MovieId;
            showTime.Price = model.Price;

            _unitOfWork.ShowTimes.Update(showTime);
            _unitOfWork.Save();

            TempData["success"] = "Show Time updated successfully!";
            return RedirectToAction("ShowTimeManagement");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShowTime([FromBody]int showTimeId)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == showTimeId);

            if (showTime != null)
            {
                try
                {
                    _unitOfWork.ShowTimes.Remove(showTime);
                    _unitOfWork.Save();


                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

            }
            return Json(new { success = false, message = "Show Time is Null!" });
        }
    }
}
