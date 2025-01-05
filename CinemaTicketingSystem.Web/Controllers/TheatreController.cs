using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class TheatreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TheatreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageTheatre()
        {
            var theatres = _unitOfWork.Theatres.GetAll().Take(9).ToList();
            ViewData["CurrentPage"] = 1;
            ViewData["TotalPages"] = (int)Math.Ceiling(_unitOfWork.Theatres.GetAll().Count() / 9.0);
            return View(theatres);
        }

        public IActionResult Details(int id)
        {
            var theatre = _unitOfWork.Theatres.Get(x=> x.TheatreId == id);
            return View(theatre);
        }

        public IActionResult CreateTheatre()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTheatre(Theatre theatre)
        {
            if (ModelState.IsValid)
            {
                // Add the theatre to the database
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult UpdateTheatre(int theatreId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateTheatre(Theatre theatre)
        {
            if (ModelState.IsValid)
            {
                // Update the theatre in the database
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult DeleteTheatre(int id)
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult DeleteTheatre(Theatre theatre)
        {
            // Delete the theatre from the database
            return RedirectToAction("Index");
        }

    }
}