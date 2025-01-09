using Azure.Core;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class HallController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HallController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateHall()
        {
            ViewData["Theatres"] = _unitOfWork.Theatres.GetAll().ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddHall([FromBody] AddHallVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model state" });
            }
            try
            {
                Hall hall = new Hall
                {
                    HallName = model.HallName,
                    TheatreId = model.TheatreId,
                };

                _unitOfWork.Halls.Add(hall);
                _unitOfWork.Save();

                Hall hallId = _unitOfWork.Halls.Get(x => x.HallName == hall.HallName & x.TheatreId == hall.TheatreId);

                return Json(new { success = true, hallId = hallId.HallId });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddSection([FromBody] List<AddSectionSeatVM> sections)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var section in sections)
                    {
                        for (int i = 1; i <= section.NumberOfSeats; i++)
                        {
                            _unitOfWork.Seats.Add(new Seat
                            {
                                HallId = section.HallId,
                                SectionName = section.SectionName,
                                SeatNumber = i,
                                IsReserved = false
                            });
                        }
                        _unitOfWork.Save();

                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {

                    return Json(new { success = false, message = ex.Message });
                }

            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteHall([FromBody] int id)
        {
            Hall hall = _unitOfWork.Halls.Get(x => x.HallId == id);
            if (hall != null)
            {
                try
                {
                    List<Seat> seats = _unitOfWork.Seats.GetAll().Where(x => x.HallId == hall.HallId).ToList();
                    foreach (var seat in seats)
                    {
                        _unitOfWork.Seats.Remove(seat);
                    }
                    _unitOfWork.Halls.Remove(hall);
                    _unitOfWork.Save();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "Hall is Null!" });
        }

        public IActionResult EditHall(int id)
        {
            Hall hall = _unitOfWork.Halls.Get(x => x.HallId == id);
            if (hall != null)
            {
                var updatehall = new UpdateHallVM
                {
                    HallId = hall.HallId,
                    HallName = hall.HallName,
                    Sections = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
                    .GroupBy(y => y.SectionName)
                        .Select(s => new UpdateSectionVM
                        {
                            SectionName = s.Key,
                            SeatsCount = s.Count()
                        })
                    .ToList()
                };
                return View(updatehall);
            }
            return RedirectToAction("ManageTheatre", "Theatre");
        }
        [HttpPost]
        public IActionResult UpdateHallName(int HallId, string hallName)
        {
            Hall hall = _unitOfWork.Halls.Get(x => x.HallId == HallId);
            if (hall != null)
            {
                try
                {
                    hall.HallName = hallName;
                    _unitOfWork.Halls.Update(hall);
                    _unitOfWork.Save();
                    TempData["success"] = "Hall name updated successfully.";
                    return RedirectToAction("EditHall", new { id = HallId });
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error updating hall name: {ex.Message}";
                    return RedirectToAction($"EditHall + {ex.Message}");
                }
            }
            return RedirectToAction("ManageTheatre", "Theatre");
        }
    }
}
