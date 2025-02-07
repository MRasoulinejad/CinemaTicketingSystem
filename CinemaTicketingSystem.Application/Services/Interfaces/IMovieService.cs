using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieListDto> GetMoviesAsync(int skip, int take);

        Task<Movie> GetMovieByIdAsync(int id);

        Task<MovieListDto> SearchMoviesAsync(string searchTerm, int page, int pageSize);

        Task AddMovieAsync(Movie movie);

        Task UpdateMovieAsync(Movie movie);

        Task DeleteMovieAsync(int id);
    }
}
