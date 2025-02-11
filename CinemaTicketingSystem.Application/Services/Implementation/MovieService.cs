using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using System.Reflection;


namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppEnvironment _appEnvironment;

        public MovieService(IUnitOfWork unitOfWork, IAppEnvironment appEnvironment)
        {
            _unitOfWork = unitOfWork;
            _appEnvironment = appEnvironment;
        }

        public async Task AddMovieAsync(AddMovieDto model)
        {
            string posterPath = null;

            if (model.Poster != null)
            {
                var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "images/movieImages");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Poster.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, model.Poster.FileData);

                posterPath = "/images/movieImages/" + uniqueFileName;
            }

            var movie = new Movie
            {
                Title = model.Title,
                Genre = model.Genre,
                Description = model.Description,
                Duration = model.Duration,
                ReleaseDate = model.ReleaseDate,
                Poster = posterPath,
                TrailerUrl = model.TrailerUrl
            };

            _unitOfWork.Movies.Add(movie);
            _unitOfWork.Save();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = _unitOfWork.Movies.Get(m => m.MovieId == id);
            if (movie == null)
            {
                throw new Exception("Movie not found.");
            }

            // Delete the movie's poster if it exists
            if (!string.IsNullOrEmpty(movie.Poster))
            {
                var posterPath = Path.Combine(_appEnvironment.WebRootPath, movie.Poster.TrimStart('/'));
                if (File.Exists(posterPath))
                {
                    File.Delete(posterPath);
                }
            }

            _unitOfWork.Movies.Remove(movie);
            _unitOfWork.Save();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return _unitOfWork.Movies.Get(x => x.MovieId == id);
        }

        public async Task<MovieListDto> GetMoviesAsync(int skip, int take)
        {
            var movies = _unitOfWork.Movies.GetAll()
                .OrderByDescending(m => m.ReleaseDate)
                .Skip(skip)
                .Take(9)
                .ToList();

            int totalMovies = _unitOfWork.Movies.GetAll().Count();

            var movieDtos = movies.Select(m => new MovieDto
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Genre = m.Genre,
                Description = m.Description,
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                Poster = m.Poster,
                TrailerUrl = m.TrailerUrl
            }).ToList();

            return new MovieListDto
            {
                Movies = movieDtos,
                Skip = skip,
                HasMore = totalMovies > skip + take
            };
        }

        public async Task<List<Movie>> SearchMoviesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Movie>();

            return _unitOfWork.Movies.GetAll()
                .Where(m => m.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task UpdateMovieAsync(UpdateMovieDto model)
        {
            var existingMovie = _unitOfWork.Movies.Get(m => m.MovieId == model.MovieId);
            if (existingMovie == null)
            {
                throw new Exception("Movie not found.");
            }

            // Update basic properties
            existingMovie.Title = model.Title;
            existingMovie.Genre = model.Genre;
            existingMovie.Description = model.Description;
            existingMovie.Duration = model.Duration;
            existingMovie.ReleaseDate = model.ReleaseDate;
            existingMovie.TrailerUrl = model.TrailerUrl;

            // Handle new poster upload
            if (model.NewPoster != null)
            {
                var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "images/movieImages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save new poster
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewPoster.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await File.WriteAllBytesAsync(filePath, model.NewPoster.FileData);

                // Delete old poster if it exists
                if (!string.IsNullOrEmpty(model.PosterPath))
                {
                    var oldPosterPath = Path.Combine(_appEnvironment.WebRootPath, model.PosterPath.TrimStart('/'));
                    if (File.Exists(oldPosterPath))
                    {
                        File.Delete(oldPosterPath);
                    }
                }

                // Update database with new poster path
                existingMovie.Poster = "/images/movieImages/" + uniqueFileName;
            }

            _unitOfWork.Movies.Update(existingMovie);
            _unitOfWork.Save();
        }
    }

}
