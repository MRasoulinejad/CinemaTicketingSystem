using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface ITheatreService
    {
        Task<TheatreListDto> GetAllTheatresAsync();
        Task AddTheatreAsync(AddTheatreDto model);
        Task<Theatre> GetTheatreByIdAsync(int id);
        Task UpdateTheatreAsync(UpdateTheatreDto model);
        Task DeleteTheatreAsync(int id);
        Task<List<Theatre>> SearchTheatresAsync(string searchTerm);
    }
}
