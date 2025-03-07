﻿using CinemaTicketingSystem.Application.ExternalServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class SMTPEmailService : ISmtpEmailService
    {
        private readonly IConfiguration _configuration;

        public SMTPEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Retrieve SMTP settings from configuration
                var host = _configuration["SMTP:Host"];
                var port = int.Parse(_configuration["SMTP:Port"]);
                var userName = _configuration["SMTP:UserName"];
                var password = _configuration["SMTP:Password"];
                var from = _configuration["SMTP:From"];

                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(userName, password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage(from, to, subject, body)
                    {
                        IsBodyHtml = true // if sending HTML emails
                    };

                    await client.SendMailAsync(mailMessage);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

            
    }
}
