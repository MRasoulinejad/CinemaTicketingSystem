using CinemaTicketingSystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class AppEnvironment : IAppEnvironment
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AppEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string WebRootPath => _hostingEnvironment.WebRootPath;
    }
}
