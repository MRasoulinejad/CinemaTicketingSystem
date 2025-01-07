using Azure.Core;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
                    foreach(var section in sections)
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


    }
}
