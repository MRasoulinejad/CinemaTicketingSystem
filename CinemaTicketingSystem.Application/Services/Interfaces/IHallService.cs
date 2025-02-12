using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IHallService
    {
        Task<List<Hall>> GetAllHallsAsync();
        Task<List<Theatre>> GetAllTheatresAsync(); 
        Task<int> AddHallAsync(AddHallDto model);
        Task AddSectionsAsync(List<AddSectionDto> sections);
        Task UpdateHallNameAsync(int hallId, string hallName); 
        Task UpdateSectionsAsync(UpdateHallDto model);
        Task DeleteHallAsync(int id);
        Task<Hall> GetHallByIdAsync(int id);
        Task<List<Hall>> GetHallsByTheatreIdAsync(int theatreId);
        Task<List<Seat>> GetSeatsByHallIdAsync(int hallId);
    }
}
