using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Implementation
{
    public class HallService : IHallService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HallService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Hall>> GetAllHallsAsync()
        {
            return _unitOfWork.Halls.GetAll().ToList();
        }

        public async Task<List<Theatre>> GetAllTheatresAsync()
        {
            return _unitOfWork.Theatres.GetAll().ToList();
        }

        public async Task<int> AddHallAsync(AddHallDto model)
        {
            var hall = new Hall
            {
                HallName = model.HallName,
                TheatreId = model.TheatreId
            };

            _unitOfWork.Halls.Add(hall);
            _unitOfWork.Save();

            var createdHall = _unitOfWork.Halls.Get(x => x.HallName == hall.HallName && x.TheatreId == hall.TheatreId);
            return createdHall.HallId;
        }

        public async Task AddSectionsAsync(List<AddSectionDto> sections)
        {
            foreach (var section in sections)
            {
                for (int i = 1; i <= section.NumberOfSeats; i++)
                {
                    _unitOfWork.Seats.Add(new Seat
                    {
                        HallId = section.HallId,
                        SectionName = section.SectionName,
                        SeatNumber = i
                    });
                }
            }
            _unitOfWork.Save();
        }

        public async Task UpdateHallNameAsync(int hallId, string hallName)
        {
            var hall = _unitOfWork.Halls.Get(x => x.HallId == hallId);
            if (hall == null) throw new Exception("Hall not found.");

            hall.HallName = hallName;
            _unitOfWork.Halls.Update(hall);
            _unitOfWork.Save();
        }

        public async Task UpdateSectionsAsync(UpdateHallDto model)
        {
            var hall = _unitOfWork.Halls.Get(x => x.HallId == model.HallId);
            if (hall == null) throw new Exception("Hall not found.");

            hall.HallName = model.HallName;
            _unitOfWork.Halls.Update(hall);
            _unitOfWork.Save();

            var seats = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId).ToList();

            foreach (var section in model.Sections)
            {
                var existingSection = seats.GroupBy(y => y.SectionName)
                    .FirstOrDefault(s => s.Key == section.OldSectionName);

                // Rename section
                if (existingSection != null && section.SectionName != section.OldSectionName)
                {
                    foreach (var seat in seats.Where(s => s.SectionName == section.OldSectionName))
                    {
                        seat.SectionName = section.SectionName;
                        _unitOfWork.Seats.Update(seat);
                    }
                }

                // Add seats if needed
                int currentSeatCount = seats.Count(s => s.SectionName == section.SectionName);
                for (int i = currentSeatCount + 1; i <= section.SeatsCount; i++)
                {
                    _unitOfWork.Seats.Add(new Seat
                    {
                        HallId = hall.HallId,
                        SectionName = section.SectionName,
                        SeatNumber = i
                    });
                }

                // Remove excess seats
                if (currentSeatCount > section.SeatsCount)
                {
                    var excessSeats = seats.Where(s => s.SectionName == section.SectionName)
                        .OrderByDescending(s => s.SeatNumber)
                        .Take(currentSeatCount - section.SeatsCount)
                        .ToList();

                    foreach (var seat in excessSeats)
                    {
                        _unitOfWork.Seats.Remove(seat);
                    }
                }
            }

            _unitOfWork.Save();
        }

        public async Task DeleteHallAsync(int id)
        {
            var hall = _unitOfWork.Halls.Get(x => x.HallId == id);
            if (hall == null) throw new Exception("Hall not found.");

            var seats = _unitOfWork.Seats.GetAll(x => x.HallId == hall.HallId).ToList();
            foreach (var seat in seats)
            {
                _unitOfWork.Seats.Remove(seat);
            }

            _unitOfWork.Halls.Remove(hall);
            _unitOfWork.Save();
        }

        public async Task<Hall> GetHallByIdAsync(int id)
        {
            return _unitOfWork.Halls.Get(x => x.HallId == id);
        }

        public async Task<List<Hall>> GetHallsByTheatreIdAsync(int theatreId)
        {
            return _unitOfWork.Halls.GetAll(x => x.TheatreId == theatreId).ToList();
        }

        public async Task<List<Seat>> GetSeatsByHallIdAsync(int hallId)
        {
            return _unitOfWork.Seats.GetAll(x => x.HallId == hallId).ToList();
        }
    }
}
