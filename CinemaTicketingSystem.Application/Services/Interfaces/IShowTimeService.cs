using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IShowTimeService
    {
        Task<ShowTimeManagementDto> GetAllShowTimesAsync();
        Task<ShowTimeDto> GetShowTimeByIdAsync(int id);
        Task<List<object>> SearchShowTimesAsync(string query, string filterBy);
        Task<List<object>> GetHallsByTheatreAsync(int theatreId);
        Task<AddShowTimeViewDataDto> GetAddShowTimeDataAsync();
        Task AddShowTimeAsync(ShowTime model);
        Task UpdateShowTimeAsync(UpdateShowTimeDto model);
        Task<bool> DeleteShowTimeAsync(int id);
        Task<EditShowTimeVmDataDto> GetEditShowTimeDataAsync(int showTimeId);
    }
}
