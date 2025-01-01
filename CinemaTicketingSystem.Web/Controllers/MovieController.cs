using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MovieController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //var movies = _db.Movies.ToList();
            // Sample Movie Data
            var movies = new List<Movie>
        {
            new Movie { MovieId = 1, Title = "Inception", Genre = "Sci-Fi", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Poster = "/images/movie1.jpg" },
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

            var viewModel = new MovieIndexViewModel
            {
                AllMovies = movies,
                LatestMovies = latestMovies
            };



            ViewData["HeroImageUrl"] = "/images/movies-hero.jpg";
            ViewData["HeroTitle"] = "Explore Our Movie Collection";
            ViewData["HeroSubtitle"] = "Discover the latest blockbusters and timeless classics. Book your tickets now!";

            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            if (id != null)
            {
                // Sample Movie Data
                var movies = new List<Movie>
                {
                    new Movie { MovieId = 1, Title = "Inception", Genre = "Sci-Fi", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Poster = "/images/movie1.jpg", TrailerUrl = "https://www.youtube.com/embed/LifqWf0BAOA" },
                    new Movie { MovieId = 2, Title = "Interstellar", Genre = "Adventure", Duration = 169, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), Poster = "/images/movie2.jpg" },
                    new Movie { MovieId = 3, Title = "Avengers: Endgame", Genre = "Action", Duration = 181, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), Poster = "/images/movie3.jpg" },
                    new Movie { MovieId = 4, Title = "The Batman", Genre = "Action", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)), Poster = "/images/movie4.jpg" },
                    new Movie { MovieId = 5, Title = "Spider-Man: No Way Home", Genre = "Adventure", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)), Poster = "/images/movie5.jpg" },
                    new Movie { MovieId = 6, Title = "Dune", Genre = "Sci-Fi", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)), Poster = "/images/movie6.jpg" },
                    new Movie { MovieId = 7, Title = "Shang-Chi", Genre = "Action", Duration = 132, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)), Poster = "/images/movie7.jpg" },
                    new Movie { MovieId = 8, Title = "Tenet", Genre = "Sci-Fi", Duration = 150, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)), Poster = "/images/movie8.jpg" },
                    new Movie { MovieId = 9, Title = "Joker", Genre = "Drama", Duration = 122, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-9)), Poster = "/images/movie9.jpg" }
                };

                Movie movie = movies.Find(x => x.MovieId == id);
                return View(movie);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Search(string query)
        {
            if (query != null)
            {
                // Sample Movie Data
                var movies = new List<Movie>
                {
                   new Movie { MovieId = 1, Title = "Inception", Genre = "Sci-Fi", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Poster = "/images/movie1.jpg", TrailerUrl = "https://www.youtube.com/embed/LifqWf0BAOA" },
                    new Movie { MovieId = 2, Title = "Interstellar", Genre = "Adventure", Duration = 169, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), Poster = "/images/movie2.jpg" },
                    new Movie { MovieId = 3, Title = "Avengers: Endgame", Genre = "Action", Duration = 181, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), Poster = "/images/movie3.jpg" },
                    new Movie { MovieId = 4, Title = "The Batman", Genre = "Action", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)), Poster = "/images/movie4.jpg" },
                    new Movie { MovieId = 5, Title = "Spider-Man: No Way Home", Genre = "Adventure", Duration = 148, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)), Poster = "/images/movie5.jpg" },
                    new Movie { MovieId = 6, Title = "Dune", Genre = "Sci-Fi", Duration = 155, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)), Poster = "/images/movie6.jpg" },
                    new Movie { MovieId = 7, Title = "Shang-Chi", Genre = "Action", Duration = 132, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)), Poster = "/images/movie7.jpg" },
                    new Movie { MovieId = 8, Title = "Tenet", Genre = "Sci-Fi", Duration = 150, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)), Poster = "/images/movie8.jpg" },
                    new Movie { MovieId = 9, Title = "Joker", Genre = "Drama", Duration = 122, ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-9)), Poster = "/images/movie9.jpg" }
                };

                var searchResults = movies.Where(m => m.Title.ToLower().Contains(query.ToLower())).ToList();
                return View(searchResults);

            }
            return View();
        }

        public IActionResult AddMovie()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                // Add the movie to the database
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult UpdateMovie()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UpdateMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                // Update the movie in the database
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult DeleteMovie(int id)
        {
            return View();
        }
        [HttpPost]
        public IActionResult DeleteMovie(Movie movie)
        {
            // Delete the movie from the database
            return RedirectToAction("Index");
        }

    }
}
