using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;


namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task AddMovieAsync(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovieAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<MovieListDto> SearchMoviesAsync(string searchTerm, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMovieAsync(Movie movie)
        {
            throw new NotImplementedException();
        }
    }

}
