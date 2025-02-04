using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.ExternalServices
{
    public interface ISmtpEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
