using CinemaTicketingSystem.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces.Payments
{
    public interface IStripeService
    {
        Task<string> CreateStripeSessionAsync(int showTimeId, List<int> selectedSeatIds, decimal totalPrice, string domain);
        Task<PaymentResultDto> ProcessPaymentSuccessAsync(string sessionId, string userName);
        Task<PaymentResultDto> ProcessPaymentFailedAsync(string sessionId, string userName);
    }
}
