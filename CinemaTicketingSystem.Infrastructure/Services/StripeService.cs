using CinemaTicketingSystem.Application.Services.Interfaces.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using CinemaTicketingSystem.Application.Common.Interfaces;


namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public StripeService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<string> CreateStripeSessionAsync(int showTimeId, List<int> selectedSeatIds, decimal totalPrice, string domain)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "cad",
                        UnitAmount = (long)(totalPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Movie Reservation",
                            Description = $"Seats: {string.Join(", ", selectedSeatIds)}"
                        },
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = $"{domain}/Reservation/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/Reservation/PaymentFailed?sessionId={{CHECKOUT_SESSION_ID}}",
                Metadata = new Dictionary<string, string>
            {
                { "showTimeId", showTimeId.ToString() },
                { "selectedSeatIds", string.Join(",", selectedSeatIds) }
            }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<PaymentResultDto> ProcessPaymentFailedAsync(string sessionId, string userName)
        {
            try
            {
                // Initialize Stripe's session service
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                // Retrieve metadata from the Stripe session
                var showTimeId = int.Parse(session.Metadata["showTimeId"]);
                var selectedSeatIds = session.Metadata["selectedSeatIds"]
                    .Split(',').Select(int.Parse).ToList();

                var totalPrice = decimal.Parse(session.AmountTotal.ToString()) / 100; // Convert from cents to dollars
                var user = await _userManager.FindByNameAsync(userName);
                var userId = user?.Id ?? session.CustomerEmail; // Use email if user not found

                // Record each reservation with "Failed" status
                foreach (var seatId in selectedSeatIds)
                {
                    // Add entry to the Reservation table
                    var reservation = new Reservation
                    {
                        ShowTimeId = showTimeId,
                        SeatId = seatId,
                        ReservationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Status = "Failed",
                        PaymentStatus = "Failed",
                        UserId = userId
                    };
                    _unitOfWork.Reservations.Add(reservation);

                    // Save all changes
                    _unitOfWork.Save();

                    // Add failed payment data to the Payment table
                    var payment = new Payment
                    {
                        ReservationId = reservation.ReservationId,
                        Amount = totalPrice,
                        PaymentDate = reservation.ReservationDate,
                        PaymentStatus = "Failed",
                        StripeSessionId = sessionId,
                        StripePaymentIntentId = session.PaymentIntentId
                    };
                    _unitOfWork.Payments.Add(payment);

                    // Save all changes
                    _unitOfWork.Save();
                }

                // Rollback temporary reservations
                foreach (var seatId in selectedSeatIds)
                {
                    var tempReservation = _unitOfWork.TemporarySeatReservations
                        .Get(r => r.ShowTimeId == showTimeId && r.SeatId == seatId);

                    if (tempReservation != null)
                    {
                        // Remove the temporary reservation
                        _unitOfWork.TemporarySeatReservations.Remove(tempReservation);

                        // Save all changes
                        _unitOfWork.Save();
                    }
                }

                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Payment failed. Your selected seats have been released. Please try again.",
                    RedirectAction = "ProceedBookingSeat",
                    RouteValues = new Dictionary<string, string>
            {
                { "showTimeId", showTimeId.ToString() },
                { "seatCount", selectedSeatIds.Count.ToString() }
            }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessPaymentFailed: {ex.Message}");
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    RedirectAction = "FailedPaymentPage"
                };
            }
        }

        public async Task<PaymentResultDto> ProcessPaymentSuccessAsync(string sessionId, string userName)
        {
            try
            {
                // Initialize Stripe's session service
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                // Verify payment status
                if (session.PaymentStatus != "paid")
                {
                    return new PaymentResultDto { Success = false, Message = "Payment not completed.", RedirectAction = "PaymentFailed" };
                }

                // Retrieve metadata from the Stripe session
                var showTimeId = int.Parse(session.Metadata["showTimeId"]);
                var selectedSeatIds = session.Metadata["selectedSeatIds"]
                    .Split(',').Select(int.Parse).ToList();


                var user = await _userManager.FindByNameAsync(userName);
                var userId = user?.Id ?? session.CustomerEmail; // Use email if user not found
                // Create a list to store reservation IDs
                var reservationIds = new List<int>();


                // Add entries to the Reservation table
                foreach (var seatId in selectedSeatIds)
                {
                    var reservation = new Reservation
                    {
                        ShowTimeId = showTimeId,
                        SeatId = seatId,
                        ReservationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Status = "Confirmed", // Reservation status
                        PaymentStatus = "Paid", // Payment status
                        UserId = userId
                    };

                    _unitOfWork.Reservations.Add(reservation);
                    _unitOfWork.Save();

                    reservationIds.Add(reservation.ReservationId); // Collect the reservation ID


                    // Add an entry to the Payment table
                    var totalPrice = session.AmountTotal / 100.0; // Convert cents to dollars
                    var payment = new Payment
                    {
                        ReservationId = reservation.ReservationId,
                        Amount = (decimal)totalPrice,
                        PaymentDate = reservation.ReservationDate,
                        PaymentStatus = "Paid",
                        StripeSessionId = sessionId,
                        StripePaymentIntentId = session.PaymentIntentId
                    };

                    _unitOfWork.Payments.Add(payment);
                    _unitOfWork.Save();
                }

                return new PaymentResultDto
                {
                    Success = true,
                    Message = "Payment successful.",
                    RedirectAction = "SuccessPage",
                    RouteValues = new Dictionary<string, string> { { "reservationIds", string.Join(",", reservationIds) } }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessPaymentSuccess: {ex.Message}");
                return new PaymentResultDto { Success = false, Message = "Error processing payment.", RedirectAction = "PaymentFailed" };
            }
        }
    }
}
