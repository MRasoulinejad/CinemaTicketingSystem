using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.Models;
using CinemaTicketingSystem.Web.ViewModels;
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
            new Theatre { TheatreId = 1, TheatreName = "Grand Cinema", Location = "Downtown",  TheatreImage = "/images/theatre1.jpg" },
            new Theatre { TheatreId = 2, TheatreName = "Cineplex Max", Location = "City Center",  TheatreImage = "/images/theatre2.jpg" },
            new Theatre { TheatreId = 3, TheatreName = "Star Cinema", Location = "Mall Plaza", TheatreImage = "/images/theatre3.jpg" },
            new Theatre { TheatreId = 4, TheatreName = "Luxe Theatre", Location = "Uptown", TheatreImage = "/images/theatre4.jpg" },
            new Theatre { TheatreId = 5, TheatreName = "Skyline Cinema", Location = "Sky Tower", TheatreImage = "/images/theatre5.jpg" }
        };

            // take 3 random theatres
            var randomTheatres = theatres.OrderBy(t => Guid.NewGuid()).Take(3).ToList();


            // Sample Movie Data
            var movies = new List<Movie>
        {
            new Movie { MovieId = 12, Title = "Inception", Genre = "Sci-Fi", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Poster = "/images/movie1.jpg" },
            new Movie { MovieId = 2, Title = "Interstellar", Genre = "Adventure", Duration = 169, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), Poster = "/images/movie2.jpg" },
            new Movie { MovieId = 3, Title = "Avengers: Endgame", Genre = "Action", Duration = 181, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), Poster = "/images/movie3.jpg" },
            new Movie { MovieId = 4, Title = "The Batman", Genre = "Action", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)), Poster = "/images/movie4.jpg" },
            new Movie { MovieId = 5, Title = "Spider-Man: No Way Home", Genre = "Adventure", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)), Poster = "/images/movie5.jpg" },
            new Movie { MovieId = 6, Title = "Dune", Genre = "Sci-Fi", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)), Poster = "/images/movie6.jpg" },
            new Movie { MovieId = 7, Title = "Shang-Chi", Genre = "Action", Duration = 132, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)), Poster = "/images/movie7.jpg" },
            new Movie { MovieId = 8, Title = "Tenet", Genre = "Sci-Fi", Duration = 150, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)), Poster = "/images/movie8.jpg" },
            new Movie { MovieId = 9, Title = "Joker", Genre = "Drama", Duration = 122, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-9)), Poster = "/images/movie9.jpg" }
        };

            // Take the latest 9 movies
            var latestMovies = movies.OrderByDescending(m => m.ReleaseDate).Take(9).ToList();

            // Pass both to the View using a ViewModel
            var HomeVM = new HomeViewModel
            {
                RandomTheatres = randomTheatres,
                LatestMovies = latestMovies
            };



            ViewData["HeroImageUrl"] = "/images/hero-banner.jpg";
            ViewData["HeroTitle"] = "Experience Movies Like Never Before!";
            ViewData["HeroSubtitle"] = "Find your favorite movies and book tickets now.";


            return View(HomeVM);
        }

        public IActionResult Privacy()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["HeroImageUrl"] = "/images/about-hero.jpg";
            ViewData["HeroTitle"] = "About Our Cinema";
            ViewData["HeroSubtitle"] = "Learn more about our state-of-the-art facilities.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
