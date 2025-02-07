using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMovieService _movieService;

        public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment
            , IMovieService movieService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _movieService = movieService;
        }

        // Initial page load: load the first 9 movies
        public async Task< IActionResult> Index()
        {
            //int skip = 0;
            //var movies = _unitOfWork.Movies.GetAll()
            //    .OrderByDescending(m => m.ReleaseDate)
            //    .Skip(skip)
            //    .Take(9)
            //    .ToList();

            //int totalMovies = _unitOfWork.Movies.GetAll().Count();
            //var viewModel = new MovieIndexViewModel
            //{
            //    Movies = movies,
            //    Skip = skip,
            //    HasMore = totalMovies > skip + 9
            //};

            int skip = 0;
            MovieListDto dto = await _movieService.GetMoviesAsync(skip, 9);

            // Map Application DTO to Web ViewModel
            var viewModel = new MovieIndexViewModel
            {
                Movies = dto.Movies.Select(m => new Movie
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    Genre = m.Genre,
                    Duration = m.Duration,
                    ReleaseDate = m.ReleaseDate,
                    Poster = m.Poster,
                }).ToList(),
                Skip = dto.Skip,
                HasMore = dto.HasMore
            };

            ViewData["HeroImageUrl"] = "/images/movies-hero.jpg";
            ViewData["HeroTitle"] = "Explore Our Movie Collection";
            ViewData["HeroSubtitle"] = "Discover the latest blockbusters and timeless classics. Book your tickets now!";

            return View(viewModel);
        }

        // AJAX endpoint to load more movies
        public IActionResult LoadMore(int skip)
        {
            var movies = _unitOfWork.Movies.GetAll()
            .OrderByDescending(m => m.ReleaseDate)
            .Skip(skip)
            .Take(9)
            .ToList();

            int totalMovies = _unitOfWork.Movies.GetAll().Count();
            var viewModel = new MovieIndexViewModel
            {
                Movies = movies,
                Skip = skip,
                HasMore = totalMovies > skip + 9
            };

            return PartialView("_MoviesPartial", viewModel);
        }

        public IActionResult Details(int id)
        {
            if (id != null)
            {
                var movie = _unitOfWork.Movies.Get(x => x.MovieId == id);


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
                List<Movie> movies = _unitOfWork.Movies.GetAll()
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
                _unitOfWork.Movies.Add(movie);
                _unitOfWork.Save();

                return RedirectToAction("ManageMovie");
            }

            ViewData["Error"] = "Please fill all required fields correctly.";
            return View(model);
        }


        public IActionResult UpdateMovie(int id)
        {
            Movie movie = _unitOfWork.Movies.Get(x => x.MovieId == id);
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
                var existingMovie = _unitOfWork.Movies.Get(x => x.MovieId == movie.MovieId);

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
                _unitOfWork.Movies.Update(existingMovie);
                _unitOfWork.Save();

                return RedirectToAction("ManageMovie");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovie([FromBody]int id)
        {
            // 🟢 Retrieve the movie record from the repository
            Movie movie = _unitOfWork.Movies.Get(x => x.MovieId == id);

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
            _unitOfWork.Movies.Remove(movie);
            _unitOfWork.Save();

            return Json(new { success = true });
        }

    }
}
