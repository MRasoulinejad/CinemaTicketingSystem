using Azure.Core;
using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Repository;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System;
using System.Reflection.Metadata.Ecma335;
using static System.Collections.Specialized.BitVector32;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class HallController : Controller
    {
        private readonly IHallService _hallService;
        private readonly ITheatreService _theatreService;
        public HallController(IHallService hallService, ITheatreService theatreService)
        {
            _hallService = hallService;
            _theatreService = theatreService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateHall()
        {
            //ViewData["Theatres"] = _unitOfWork.Theatres.GetAll().ToList();
            //return View();
            var dto = await _theatreService.GetAllTheatresAsync();

            ViewData["Theatres"] = dto.Theatres;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddHall([FromBody] AddHallVM model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Json(new { success = false, message = "Invalid model state" });
            //}
            //try
            //{
            //    Hall hall = new Hall
            //    {
            //        HallName = model.HallName,
            //        TheatreId = model.TheatreId,
            //    };

            //    _unitOfWork.Halls.Add(hall);
            //    _unitOfWork.Save();

            //    Hall hallId = _unitOfWork.Halls.Get(x => x.HallName == hall.HallName & x.TheatreId == hall.TheatreId);

            //    return Json(new { success = true, hallId = hallId.HallId });
            //}
            //catch (Exception ex)
            //{

            //    return Json(new { success = false, message = ex.Message });
            //}

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model state" });
            }

            try
            {
                int hallId = await _hallService.AddHallAsync(new AddHallDto
                {
                    HallName = model.HallName,
                    TheatreId = model.TheatreId
                });

                return Json(new { success = true, hallId = hallId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSection([FromBody] List<AddSectionSeatVM> sections)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        foreach (var section in sections)
            //        {
            //            for (int i = 1; i <= section.NumberOfSeats; i++)
            //            {
            //                _unitOfWork.Seats.Add(new Seat
            //                {
            //                    HallId = section.HallId,
            //                    SectionName = section.SectionName,
            //                    SeatNumber = i,
            //                    //IsReserved = false
            //                });
            //            }
            //            _unitOfWork.Save();

            //        }

            //        return Json(new { success = true });
            //    }
            //    catch (Exception ex)
            //    {

            //        return Json(new { success = false, message = ex.Message });
            //    }

            //}
            //return Json(new { success = false });

            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            try
            {
                await _hallService.AddSectionsAsync(sections.Select(s => new AddSectionDto
                {
                    HallId = s.HallId,
                    SectionName = s.SectionName,
                    NumberOfSeats = s.NumberOfSeats
                }).ToList());

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHall([FromBody] int id)
        {
            //Hall hall = _unitOfWork.Halls.Get(x => x.HallId == id);
            //if (hall != null)
            //{
            //    try
            //    {
            //        List<Seat> seats = _unitOfWork.Seats.GetAll().Where(x => x.HallId == hall.HallId).ToList();
            //        foreach (var seat in seats)
            //        {
            //            _unitOfWork.Seats.Remove(seat);
            //        }
            //        _unitOfWork.Halls.Remove(hall);
            //        _unitOfWork.Save();
            //        return Json(new { success = true });
            //    }
            //    catch (Exception ex)
            //    {
            //        return Json(new { success = false, message = ex.Message });
            //    }
            //}
            //return Json(new { success = false, message = "Hall is Null!" });

            try
            {
                await _hallService.DeleteHallAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> EditHall(int id)
        {
            //Hall hall = _unitOfWork.Halls.Get(x => x.HallId == id);
            //if (hall != null)
            //{
            //    var updatehall = new EditHallVM
            //    {
            //        HallId = hall.HallId,
            //        HallName = hall.HallName,
            //        Sections = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
            //        .GroupBy(y => y.SectionName)
            //            .Select(s => new EditSectionVM
            //            {
            //                SectionName = s.Key,
            //                SeatsCount = s.Count()
            //            })
            //        .ToList()
            //    };
            //    return View(updatehall);
            //}
            //return RedirectToAction("ManageTheatre", "Theatre");

            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return RedirectToAction("ManageTheatre", "Theatre");
            }

            var editHallVM = new EditHallVM
            {
                HallId = hall.HallId,
                HallName = hall.HallName,
                Sections = _hallService.GetSeatsByHallIdAsync(hall.HallId).Result
                    .GroupBy(y => y.SectionName)
                    .Select(s => new EditSectionVM
                    {
                        SectionName = s.Key,
                        SeatsCount = s.Count()
                    })
                    .ToList()
            };

            return View(editHallVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHallName(int hallId, string hallName)
        {
            //Hall hall = _unitOfWork.Halls.Get(x => x.HallId == HallId);
            //if (hall != null)
            //{
            //    try
            //    {
            //        hall.HallName = hallName;
            //        _unitOfWork.Halls.Update(hall);
            //        _unitOfWork.Save();
            //        TempData["success"] = "Hall name updated successfully.";
            //        return RedirectToAction("EditHall", new { id = HallId });
            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["error"] = $"Error updating hall name: {ex.Message}";
            //        return RedirectToAction($"EditHall + {ex.Message}");
            //    }
            //}
            //return RedirectToAction("ManageTheatre", "Theatre");
            try
            {
                await _hallService.UpdateHallNameAsync(hallId, hallName);
                TempData["success"] = "Hall name updated successfully.";
                return RedirectToAction("EditHall", new { id = hallId });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error updating hall name: {ex.Message}";
                return RedirectToAction("EditHall", new { id = hallId });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSections(EditHallVM model)
        {
            //Hall hall = _unitOfWork.Halls.Get(x => x.HallId == model.HallId);

            //if (hall is not null)
            //{
            //    var updatehall = new EditHallVM
            //    {
            //        HallId = hall.HallId,
            //        HallName = hall.HallName,
            //        Sections = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId)
            //            .GroupBy(y => y.SectionName)
            //            .Select(s => new EditSectionVM
            //            {
            //                SectionName = s.Key,
            //                SeatsCount = s.Count()
            //            })
            //            .ToList()
            //    };

            //    List<Seat> seats = _unitOfWork.Seats
            //        .GetAll(x => x.HallId == hall.HallId)
            //        .ToList();

            //    foreach (var item in model.Sections)
            //    {
            //        // Handle section name changes
            //        if (updatehall.Sections.Any(x => x.SectionName == item.OldSectionName && item.SectionName != item.OldSectionName))
            //        {
            //            foreach (var seat in seats.Where(s => s.SectionName == item.OldSectionName))
            //            {
            //                seat.SectionName = item.SectionName;
            //                _unitOfWork.Seats.Update(seat);
            //            }
            //            _unitOfWork.Save();
            //        }

            //        // Handle seat count increases
            //        var existingSection = updatehall.Sections.FirstOrDefault(x => x.SectionName == item.SectionName);
            //        if (existingSection != null && existingSection.SeatsCount < item.SeatsCount)
            //        {
            //            int currentSeatCount = seats.Count(s => s.SectionName == item.SectionName);
            //            for (int i = currentSeatCount + 1; i <= item.SeatsCount; i++)
            //            {
            //                _unitOfWork.Seats.Add(new Seat
            //                {
            //                    HallId = hall.HallId,
            //                    SectionName = item.SectionName,
            //                    SeatNumber = i,
            //                    //IsReserved = false
            //                });
            //            }
            //            _unitOfWork.Save();
            //        }

            //        // Handle seat count decreases
            //        if (existingSection != null && existingSection.SeatsCount > item.SeatsCount)
            //        {
            //            var excessSeats = seats
            //                .Where(s => s.SectionName == item.SectionName)
            //                .OrderByDescending(s => s.SeatNumber)
            //                .Take(existingSection.SeatsCount - item.SeatsCount)
            //                .ToList();

            //            foreach (var seatToDelete in excessSeats)
            //            {
            //                _unitOfWork.Seats.Remove(seatToDelete);
            //            }
            //            _unitOfWork.Save();
            //        }
            //    }

            //    TempData["success"] = "Sections updated successfully.";
            //    return RedirectToAction("EditHall", new { id = hall.HallId });
            //}

            //// If hall is null retun to view
            //return RedirectToAction("EditHall", model);

            try
            {
                Hall hall = _hallService.GetHallByIdAsync(model.HallId).Result;

                await _hallService.UpdateSectionsAsync(new UpdateHallDto
                {
                    HallId = model.HallId,
                    HallName = hall.HallName,
                    Sections = model.Sections.Select(s => new UpdateSectionDto
                    {
                        OldSectionName = s.OldSectionName,
                        SectionName = s.SectionName,
                        SeatsCount = s.SeatsCount
                    }).ToList()
                });

                TempData["success"] = "Sections updated successfully.";
                return RedirectToAction("EditHall", new { id = model.HallId });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error updating sections: {ex.Message}";
                return RedirectToAction("EditHall", new { id = model.HallId });
            }
        }

    }
}