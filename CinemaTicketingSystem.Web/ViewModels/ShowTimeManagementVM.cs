using CinemaTicketingSystem.Domain.Entities;

namespace CinemaTicketingSystem.Web.ViewModels
{
    public class ShowTimeManagementVM
    {
        public List<ShowTime> ShowTimes { get; set; }
        public int TotalShowTimes { get; set; }
        public int TotalTheatres { get; set; }
        public int TotalMovies { get; set; }
        public double TotalMinutes { get; set; }
    }
}
