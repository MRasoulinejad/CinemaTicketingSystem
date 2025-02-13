using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowTimeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShowTimeManagementDto> GetAllShowTimesAsync()
        {
            // do not need these
            var showTimes = _unitOfWork.ShowTimes.GetAll(includeProperties: "Theatre,Movie").ToList();
            var showTimeCount = showTimes.Count();
            var theatreCount = showTimes.Select(s => s.TheatreId).Distinct().Count();
            var movieCount = showTimes.Select(s => s.MovieId).Distinct().Count();
            var totalMinutes = showTimes.Sum(s => (s.ShowTimeEnd - s.ShowTimeStart).TotalMinutes);

            var model = new ShowTimeManagementDto
            {
                ShowTimes = showTimes,
                TotalShowTimes = showTimes.Count,
                TotalTheatres = showTimes.Select(s => s.TheatreId).Distinct().Count(),
                TotalMovies = showTimes.Select(s => s.MovieId).Distinct().Count(),
                TotalMinutes = showTimes.Sum(s => (s.ShowTimeEnd - s.ShowTimeStart).TotalMinutes)
            };

            return model;
        }

        public async Task<ShowTimeDto> GetShowTimeByIdAsync(int id)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == id);
            if (showTime == null) return null;

            return new ShowTimeDto
            {
                ShowTimeId = showTime.ShowTimeId,
                ShowDate = showTime.ShowDate,
                ShowTimeStart = showTime.ShowTimeStart,
                ShowTimeEnd = showTime.ShowTimeEnd,
                TheatreId = showTime.TheatreId,
                MovieId = showTime.MovieId,
                HallId = showTime.HallId,
                Price = showTime.Price
            };
        }

        public async Task<List<object>> SearchShowTimesAsync(string query, string filterBy)
        {
            //List<int> relevantIds = new List<int>();

            //switch (filterBy.ToLower())
            //{
            //    case "movie":
            //        relevantIds = _unitOfWork.Movies.GetAll()
            //            .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            //            .Select(m => m.MovieId)
            //            .ToList();
            //        break;

            //    case "theatre":
            //        relevantIds = _unitOfWork.Theatres.GetAll()
            //            .Where(t => t.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase))
            //            .Select(t => t.TheatreId)
            //            .ToList();
            //        break;

            //    default:
            //        throw new ArgumentException("Invalid filterBy value. Use 'movie' or 'theatre'.");
            //}

            //var showTimes = filterBy.ToLower() == "movie"
            //    ? _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.MovieId)).ToList()
            //    : _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.TheatreId)).ToList();

            //return showTimes.Select(s => new ShowTimeDto
            //{
            //    ShowTimeId = s.ShowTimeId,
            //    ShowDate = s.ShowDate,
            //    ShowTimeStart = s.ShowTimeStart,
            //    ShowTimeEnd = s.ShowTimeEnd,
            //    TheatreId = s.TheatreId,
            //    MovieId = s.MovieId,
            //    HallId = s.HallId,
            //    Price = s.Price
            //}).ToList();

            List<int> relevantIds = new List<int>();

            // Filter based on the criteria
            switch (filterBy.ToLower())
            {
                case "movie":
                    // Fetch relevant MovieIds
                    relevantIds = _unitOfWork.Movies
                        .GetAll()
                        .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Select(m => m.MovieId)
                        .ToList();
                    break;

                case "theatre":
                    // Fetch relevant TheatreIds
                    relevantIds = _unitOfWork.Theatres
                        .GetAll()
                        .Where(t => t.TheatreName.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.TheatreId)
                        .ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid filterBy value. Use 'movie' or 'theatre'.");
            }

            // Fetch ShowTimes based on the filtered IDs
            var showTimes = filterBy.ToLower() == "movie"
                ? _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.MovieId))
                : _unitOfWork.ShowTimes.GetAll().Where(s => relevantIds.Contains(s.TheatreId));


            // Project the result
            var result = showTimes.Select(s => new
            {
                s.ShowTimeId,
                ShowDate = s.ShowDate.ToShortDateString(),
                StartTime = s.ShowTimeStart.ToString(@"hh\:mm"),
                EndTime = s.ShowTimeEnd.ToString(@"hh\:mm"),
                Theatre = _unitOfWork.Theatres.GetAll().FirstOrDefault(t => t.TheatreId == s.TheatreId)?.TheatreName ?? "N/A",
                Hall = _unitOfWork.Halls.GetAll().FirstOrDefault(h => h.HallId == s.HallId)?.HallName ?? "N/A",
                Movie = _unitOfWork.Movies.GetAll().FirstOrDefault(t => t.MovieId == s.MovieId)?.Title ?? "N/A",
                Price = s.Price
            }).ToList();

            return result.Cast<object>().ToList();
        }

        public async Task<List<object>> GetHallsByTheatreAsync(int theatreId)
        {
            var halls = _unitOfWork.Halls
                .GetAll(h => h.TheatreId == theatreId)
                .Select(h => new { h.HallId, h.HallName })
                .ToList();

            return halls.Cast<object>().ToList();

        }

        public async Task<AddShowTimeViewDataDto> GetAddShowTimeDataAsync()
        {
            return new AddShowTimeViewDataDto
            {
                Theatres = _unitOfWork.Theatres.GetAll().ToList(),
                Movies = _unitOfWork.Movies.GetAll().ToList()
            };
        }

        public async Task AddShowTimeAsync(ShowTime model)
        {
            var showTime = new ShowTime
            {
                ShowDate = model.ShowDate,
                ShowTimeStart = model.ShowTimeStart,
                ShowTimeEnd = model.ShowTimeEnd,
                TheatreId = model.TheatreId,
                MovieId = model.MovieId,
                HallId = model.HallId,
                Price = model.Price
            };

            _unitOfWork.ShowTimes.Add(showTime);
            _unitOfWork.Save();
        }

        public async Task<bool> DeleteShowTimeAsync(int id)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == id);
            if (showTime == null) return false;

            _unitOfWork.ShowTimes.Remove(showTime);
            _unitOfWork.Save();
            return true;
        }

        public async Task UpdateShowTimeAsync(UpdateShowTimeDto model)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == model.ShowTimeId);
            if (showTime == null) throw new Exception("ShowTime not found.");

            // Update properties
            showTime.ShowDate = model.ShowDate;
            showTime.ShowTimeStart = model.ShowTimeStart;
            showTime.ShowTimeEnd = model.ShowTimeEnd;
            showTime.TheatreId = model.TheatreId;
            showTime.HallId = model.HallId;
            showTime.MovieId = model.MovieId;
            showTime.Price = model.Price;

            // Save changes
            _unitOfWork.ShowTimes.Update(showTime);
            _unitOfWork.Save();
        }

        public Task<EditShowTimeVmDataDto> GetEditShowTimeDataAsync(int showTimeId)
        {
            var showTime = _unitOfWork.ShowTimes.Get(s => s.ShowTimeId == showTimeId);
            if (showTime == null) throw new Exception("ShowTime not found.");

            var hall = _unitOfWork.Halls.Get(x => x.HallId == showTime.HallId);

            return Task.FromResult(new EditShowTimeVmDataDto
            {
                ShowTimeId = showTime.ShowTimeId,
                ShowDate = showTime.ShowDate,
                ShowTimeStart = showTime.ShowTimeStart,
                ShowTimeEnd = showTime.ShowTimeEnd,
                TheatreId = showTime.TheatreId,
                MovieId = showTime.MovieId,
                Price = showTime.Price,
                Theatres = _unitOfWork.Theatres.GetAll().ToList(),
                Movies = _unitOfWork.Movies.GetAll().ToList(),
                HallId = hall.HallId,
                HallName = hall.HallName
            });
        }
    }
}
