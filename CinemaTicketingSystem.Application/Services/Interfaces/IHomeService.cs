using CinemaTicketingSystem.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeVmDto> GetDataForIndexAsync();
        Task<bool> SendContactEmailAsync(ContactFormVmDto model);
    }
}
