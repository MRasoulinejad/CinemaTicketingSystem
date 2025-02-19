using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectAction { get; set; }
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
    }
}
