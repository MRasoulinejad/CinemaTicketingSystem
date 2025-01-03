using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieController(IMovieRepository movieRepository, IWebHostEnvironment webHostEnvironment)
        {
            _movieRepository = movieRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            //var movies = _movieRepository.GetAll();
            //var latestMovies = movies.OrderByDescending(m => m.ReleaseDate).Take(9).ToList();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search([FromBody] string searchTerm, int page = 1)
        {
            const int PageSize = 9;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Get all movies
                List<Movie> movies = _movieRepository.GetAll()
                    .Where(x => x.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var paginatedMovies = movies
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

                ViewData["CurrentPage"] = page;
                ViewData["TotalPages"] = (int)Math.Ceiling((double)movies.Count / PageSize);


                return PartialView("_MovieListPartial", paginatedMovies);
            }

            return PartialView("_MovieListPartial", new List<Movie>());
        }

        public IActionResult ManageMovie()
        {
            return View();
        }

        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(AddMovieVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.Poster != null)
                {
                    // 📂 define the uploads folder path
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/movieImages");

                    // 🛡️ make sure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 📝 make the file name unique
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Poster.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 💾 save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Poster.CopyTo(fileStream);
                    }
                }

                // 🛠️ map the view model to the domain model
                var movie = new Movie
                {
                    Title = model.Title,
                    Genre = model.Genre,
                    Description = model.Description,
                    Duration = model.Duration,
                    ReleaseDate = model.ReleaseDate,
                    Poster = uniqueFileName != null ? "/images/movieImages/" + uniqueFileName : null,
                    TrailerUrl = model.TrailerUrl
                };

                // 🗃️ Add the movie to the database
                _movieRepository.Add(movie);
                _movieRepository.Save();

                return RedirectToAction("ManageMovie");
            }

            ViewData["Error"] = "Please fill all required fields correctly.";
            return View(model);
        }


        public IActionResult UpdateMovie(int id)
        {
            Movie movie = _movieRepository.Get(x => x.MovieId == id);
            UpdateMovieVM updateMovieVM = new UpdateMovieVM
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Genre = movie.Genre,
                Description = movie.Description,
                Duration = movie.Duration,
                ReleaseDate = movie.ReleaseDate,
                Poster = movie.Poster,
                TrailerUrl = movie.TrailerUrl
            };

            return View(updateMovieVM);
        }

        [HttpPost]
        public IActionResult UpdateMovie(UpdateMovieVM movie)
        {

            if (ModelState.IsValid)
            {
                // Retrieve the existing movie from the database
                var existingMovie = _movieRepository.Get(x => x.MovieId == movie.MovieId);

                if (existingMovie == null)
                {
                    return NotFound();
                }

                // Update basic properties
                existingMovie.Title = movie.Title;
                existingMovie.Genre = movie.Genre;
                existingMovie.Description = movie.Description;
                existingMovie.Duration = movie.Duration;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.TrailerUrl = movie.TrailerUrl;

                // Handle poster upload if a new file is provided
                if (movie.PosterFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movieImages");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.PosterFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        movie.PosterFile.CopyTo(fileStream);
                    }

                    // Delete old poster if it exists
                    if (!string.IsNullOrEmpty(existingMovie.Poster))
                    {
                        var oldPosterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingMovie.Poster.TrimStart('/'));
                        if (System.IO.File.Exists(oldPosterPath))
                        {
                            System.IO.File.Delete(oldPosterPath);
                        }
                    }

                    // Update the new poster path
                    existingMovie.Poster = "/images/movieImages/" + uniqueFileName;
                }

                // Save the updated movie
                _movieRepository.Update(existingMovie);
                _movieRepository.Save();

                return RedirectToAction("ManageMovie");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovie([FromBody]int id)
        {
            // 🟢 Retrieve the movie record from the repository
            Movie movie = _movieRepository.Get(x => x.MovieId == id);

            if (movie == null)
            {
                return Json(new { success = false, message = "Movie not found." });
            }

            // 🖼️ Delete the poster file if it exists
            if (!string.IsNullOrEmpty(movie.Poster))
            {
                // Ensure the path starts without a slash
                var posterRelativePath = movie.Poster.TrimStart('/');

                // Build the absolute path to the poster
                var posterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", posterRelativePath);

                if (System.IO.File.Exists(posterPath))
                {
                    try
                    {
                        System.IO.File.Delete(posterPath);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Failed to delete poster: {ex.Message}" });
                    }
                }
            }

            // 🗑️ Remove the movie record from the database
            _movieRepository.Remove(movie);
            _movieRepository.Save();

            return Json(new { success = true });
        }

    }
}
