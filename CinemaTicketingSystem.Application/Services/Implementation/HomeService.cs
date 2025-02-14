using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmtpEmailService _emailService;
        public HomeService(IUnitOfWork unitOfWork, ISmtpEmailService smtpEmailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = smtpEmailService;
        }

        public async Task<HomeVmDto> GetDataForIndexAsync()
        {
            // Take 3 Random Theatres
            var theatres = _unitOfWork.Theatres.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();


            // Take 9 Random Movies
            var movies = _unitOfWork.Movies.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(9)
                .ToList();

            var homeVm = new HomeVmDto
            {
                RandomTheatres = theatres,
                LatestMovies = movies
            };

            return homeVm;
        }

        public async Task<bool> SendContactEmailAsync(ContactFormVmDto model)
        {
            try
            {
                string subject = $"Contact Form Submission: {model.Subject}";
                string body = $"Message from: {model.Name} ({model.Email})<br/><br/>{model.Message}";
                bool result = await _emailService.SendEmailAsync(model.AdminEmail, subject, body);

                if (result)
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
