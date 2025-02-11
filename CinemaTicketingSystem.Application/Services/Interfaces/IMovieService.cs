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

        Task<List<Movie>> SearchMoviesAsync(string searchTerm);

        Task AddMovieAsync(AddMovieDto model);

        Task UpdateMovieAsync(UpdateMovieDto model);

        Task DeleteMovieAsync(int id);
    }
}
