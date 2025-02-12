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
    public class TheatreService : ITheatreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppEnvironment _appEnvironment; // Used for WebRootPath

        public TheatreService(IUnitOfWork unitOfWork, IAppEnvironment appEnvironment)
        {
            _unitOfWork = unitOfWork;
            _appEnvironment = appEnvironment;
        }

        public async Task<TheatreListDto> GetAllTheatresAsync()
        {
            var theatres = _unitOfWork.Theatres.GetAll().ToList();
            return new TheatreListDto { Theatres = theatres };
        }

        public async Task AddTheatreAsync(AddTheatreDto model)
        {
            string imagePath = null;

            if (model.TheatreImage != null)
            {
                var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "images/TheatreImages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.TheatreImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await File.WriteAllBytesAsync(filePath, model.TheatreImage.FileData);

                imagePath = "/images/TheatreImages/" + uniqueFileName;
            }

            var theatre = new Theatre
            {
                TheatreName = model.TheatreName,
                Location = model.Location,
                Description = model.Description,
                TheatreImage = imagePath
            };

            _unitOfWork.Theatres.Add(theatre);
            _unitOfWork.Save();
        }

        public async Task<Theatre> GetTheatreByIdAsync(int id)
        {
            return _unitOfWork.Theatres.Get(x => x.TheatreId == id);

        }

        public async Task UpdateTheatreAsync(UpdateTheatreDto model)
        {
            var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == model.TheatreId);
            if (theatre == null) throw new Exception("Theatre not found.");

            theatre.TheatreName = model.TheatreName;
            theatre.Location = model.Location;
            theatre.Description = model.Description;

            if (model.NewImage != null)
            {
                var uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "images/TheatreImages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await File.WriteAllBytesAsync(filePath, model.NewImage.FileData);

                // Delete old image
                if (!string.IsNullOrEmpty(model.CurrentImage))
                {
                    var oldImagePath = Path.Combine(_appEnvironment.WebRootPath, model.CurrentImage.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                theatre.TheatreImage = "/images/TheatreImages/" + uniqueFileName;
            }

            _unitOfWork.Theatres.Update(theatre);
            _unitOfWork.Save();
        }

        public async Task DeleteTheatreAsync(int id)
        {
            var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == id);
            if (theatre == null) throw new Exception("Theatre not found.");

            // Delete related Halls and Seats
            var halls = _unitOfWork.Halls.GetAll(h => h.TheatreId == theatre.TheatreId).ToList();
            foreach (var hall in halls)
            {
                var seats = _unitOfWork.Seats.GetAll(s => s.HallId == hall.HallId).ToList();
                foreach (var seat in seats)
                {
                    _unitOfWork.Seats.Remove(seat);
                }
                _unitOfWork.Halls.Remove(hall);
            }

            // Delete Theatre Image
            if (!string.IsNullOrEmpty(theatre.TheatreImage))
            {
                var imagePath = Path.Combine(_appEnvironment.WebRootPath, theatre.TheatreImage.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            _unitOfWork.Theatres.Remove(theatre);
            _unitOfWork.Save();
        }

        public async Task<List<Theatre>> SearchTheatresAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<Theatre>();

            return _unitOfWork.Theatres.GetAll()
                .Where(x => x.TheatreName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
