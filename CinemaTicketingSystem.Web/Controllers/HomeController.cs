using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var theatres = new List<Theatre>
        {
            new Theatre { TheatreId = 1, TheatreName = "Grand Cinema", Location = "Downtown", TotalSeats = 150, ImageUrl = "/images/theatre1.jpg" },
            new Theatre { TheatreId = 2, TheatreName = "Cineplex Max", Location = "City Center", TotalSeats = 200, ImageUrl = "/images/theatre2.jpg" },
            new Theatre { TheatreId = 3, TheatreName = "Star Cinema", Location = "Mall Plaza", TotalSeats = 180, ImageUrl = "/images/theatre3.jpg" },
            new Theatre { TheatreId = 4, TheatreName = "Luxe Theatre", Location = "Uptown", TotalSeats = 120, ImageUrl = "/images/theatre4.jpg" },
            new Theatre { TheatreId = 5, TheatreName = "Skyline Cinema", Location = "Sky Tower", TotalSeats = 300, ImageUrl = "/images/theatre5.jpg" }
        };

            // take 3 random theatres
            var randomTheatres = theatres.OrderBy(t => Guid.NewGuid()).Take(3).ToList();




            return View(randomTheatres);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
