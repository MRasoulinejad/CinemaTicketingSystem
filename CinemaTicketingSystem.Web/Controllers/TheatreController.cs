using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class TheatreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
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