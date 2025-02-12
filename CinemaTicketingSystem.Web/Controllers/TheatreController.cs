using CinemaTicketingSystem.Application.Common.DTO;
using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Repository;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class TheatreController : Controller
    {
        private readonly ITheatreService _theatreService;
        private readonly IHallService _hallService;

        public TheatreController(ITheatreService theatreService, IHallService hallService)
        {
            _theatreService = theatreService;
            _hallService = hallService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageTheatre()
        {
            return View();
        }

        [Route("Theatre/AllTheatres")]
        public async Task<IActionResult> GetAllTheatres()
        {
            //var theatres = _unitOfWork.Theatres.GetAll().ToList();
            //return View(theatres);

            var dto = await _theatreService.GetAllTheatresAsync();
            return View(dto.Theatres);

        }

        public IActionResult CreateTheatre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTheatre(AddTheatreVM model)
        {
            //if (ModelState.IsValid)
            //{
            //    string uniqueFileName = null;

            //    if (model.TheatreImage != null)
            //    {
            //        // 📂 define the uploads folder path
            //        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/TheatreImages");

            //        // 🛡️ make sure the folder exists
            //        if (!Directory.Exists(uploadsFolder))
            //        {
            //            Directory.CreateDirectory(uploadsFolder);
            //        }

            //        // 📝 make the file name unique
            //        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.TheatreImage.FileName;
            //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //        // 💾 save the file to the server
            //        using (var fileStream = new FileStream(filePath, FileMode.Create))
            //        {
            //            model.TheatreImage.CopyTo(fileStream);
            //        }
            //    }

            //    // 🛠️ map the view model to the domain model
            //    var theatre = new Theatre
            //    {
            //        TheatreName = model.TheatreName,
            //        Location = model.Location,
            //        Description = model.Description,
            //        TheatreImage = uniqueFileName != null ? "/images/TheatreImages/" + uniqueFileName : null
            //    };

            //    // 🗃️ Add the movie to the database
            //    _unitOfWork.Theatres.Add(theatre);
            //    _unitOfWork.Save();

            //    return RedirectToAction("ManageTheatre");
            //}

            //ViewData["Error"] = "Please fill all required fields correctly.";
            //return View(model);


            if (ModelState.IsValid)
            {
                var dto = new AddTheatreDto
                {
                    TheatreName = model.TheatreName,
                    Location = model.Location,
                    Description = model.Description,
                    TheatreImage = model.TheatreImage != null ? ConvertIFormFileToDto(model.TheatreImage) : null
                };

                await _theatreService.AddTheatreAsync(dto);
                return RedirectToAction("ManageTheatre");
            }

            ViewData["Error"] = "Please fill all required fields correctly.";
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromBody] string searchTerm)
        {

            //if (!string.IsNullOrWhiteSpace(searchTerm))
            //{
            //    // Get all movies
            //    List<Theatre> theatres = _unitOfWork.Theatres.GetAll()
            //        .Where(x => x.TheatreName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            //        .ToList();

            //    return PartialView("_TheatreListPartial", theatres);
            //}

            //return PartialView("_TheatreListPartial", new List<Theatre>());

            var theatres = await _theatreService.SearchTheatresAsync(searchTerm);
            return PartialView("_TheatreListPartial", theatres);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTheatre(int id)
        {
            //var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == id);

            //var halls = _unitOfWork.Halls.GetAll(h => h.TheatreId == theatre.TheatreId)
            //    .ToList();

            //var theatreVM = new UpdateTheatreVM
            //{
            //    TheatreId = theatre.TheatreId,
            //    TheatreName = theatre.TheatreName,
            //    Location = theatre.Location,
            //    Description = theatre.Description,
            //    CurrentImage = theatre.TheatreImage,
            //    Halls = halls.Select(h => new UpdateHallVM
            //    {
            //        HallId = h.HallId,
            //        HallName = h.HallName,
            //        Sections = _unitOfWork.Seats.GetAll(b => b.HallId == h.HallId)
            //            .GroupBy(y => y.SectionName)
            //            .Select(s => new UpdateSectionVM
            //            {
            //                SectionName = s.Key,
            //                SeatsCount = s.Count()
            //            })
            //            .ToList()
            //    }).ToList()
            //};
            //return View(theatreVM);


            var theatre = await _theatreService.GetTheatreByIdAsync(id);
            if (theatre == null)
            {
                return NotFound();
            }

            var halls = await _hallService.GetHallsByTheatreIdAsync(theatre.TheatreId);

            var theatreVM = new UpdateTheatreVM
            {
                TheatreId = theatre.TheatreId,
                TheatreName = theatre.TheatreName,
                Location = theatre.Location,
                Description = theatre.Description,
                CurrentImage = theatre.TheatreImage,
                Halls = halls.Select(h => new UpdateHallVM
                {
                    HallId = h.HallId,
                    HallName = h.HallName,
                    Sections = _hallService.GetSeatsByHallIdAsync(h.HallId).Result
                        .GroupBy(s => s.SectionName)
                        .Select(s => new UpdateSectionVM
                        {
                            SectionName = s.Key,
                            SeatsCount = s.Count()
                        }).ToList()
                    
                }).ToList()
            };

            return View(theatreVM);

        }

        public async Task<IActionResult> UpdateTheatreSecondStep(int id)
        {
            //var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == id);
            //if (theatre != null )
            //{
            //    try
            //    {
            //        var theatreVM = new UpdateTheatreVM
            //        {
            //            TheatreId = theatre.TheatreId,
            //            TheatreName = theatre.TheatreName,
            //            Location = theatre.Location,
            //            Description = theatre.Description,
            //            CurrentImage = theatre.TheatreImage,
            //        };
            //        return View(theatreVM);
            //    }
            //    catch (Exception ex)
            //    {
            //        return RedirectToAction($"UpdateTheatre + {ex.Message}");
            //    } 
            //}
            //return RedirectToAction("UpdateTheatre");

            var theatre = await _theatreService.GetTheatreByIdAsync(id);
            if (theatre == null)
            {
                return RedirectToAction("UpdateTheatre");
            }

            var theatreVM = new UpdateTheatreVM
            {
                TheatreId = theatre.TheatreId,
                TheatreName = theatre.TheatreName,
                Location = theatre.Location,
                Description = theatre.Description,
                CurrentImage = theatre.TheatreImage,
            };

            return View(theatreVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTheatreSecondStep(UpdateTheatreSecondStepVM model)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {

            //        // 🛠️ map the view model to the domain model
            //        var theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == model.TheatreId);
            //        theatre.TheatreName = model.TheatreName;
            //        theatre.Location = model.Location;
            //        theatre.Description = model.Description;

            //        string uniqueFileName = null;
            //        if (model.TheatreImage != null)
            //        {
            //            // 📂 define the uploads folder path
            //            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/TheatreImages");
            //            // 🛡️ make sure the folder exists
            //            if (!Directory.Exists(uploadsFolder))
            //            {
            //                Directory.CreateDirectory(uploadsFolder);
            //            }
            //            // 📝 make the file name unique
            //            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.TheatreImage.FileName;
            //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //            // 💾 save the file to the server
            //            using (var fileStream = new FileStream(filePath, FileMode.Create))
            //            {
            //                model.TheatreImage.CopyTo(fileStream);
            //            }
            //            theatre.TheatreImage = uniqueFileName != null ? "/images/TheatreImages/" + uniqueFileName : null;
            //        }

            //        // 🗃️ Add the theatre to the database
            //        _unitOfWork.Theatres.Update(theatre);
            //        _unitOfWork.Save();
            //        ViewData["success"] = "Theatre successfully updated.";
            //        return RedirectToAction("ManageTheatre");
            //    }
            //    catch (Exception ex)
            //    {
            //        return RedirectToAction($"UpdateTheatre + {ex.Message}");
            //    }

            //}
            //ViewData["Error"] = "Please fill all required fields correctly.";
            //return View(model);

            if (ModelState.IsValid)
            {
                var dto = new UpdateTheatreDto
                {
                    TheatreId = model.TheatreId,
                    TheatreName = model.TheatreName,
                    Location = model.Location,
                    Description = model.Description,
                    //CurrentImage = model.CurrentImage,
                    NewImage = model.TheatreImage != null ? ConvertIFormFileToDto(model.TheatreImage) : null
                };

                await _theatreService.UpdateTheatreAsync(dto);
                return RedirectToAction("ManageTheatre");
            }

            ViewData["Error"] = "Please fill all required fields correctly.";
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTheatre([FromBody] int id)
        {

            //Theatre theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == id);

            //if (theatre != null)
            //{
            //    try
            //    {
            //        List<Hall> halls = _unitOfWork.Halls.GetAll().Where(x => x.TheatreId == theatre.TheatreId).ToList();
            //        foreach (var hall in halls)
            //        {
            //            List<Seat> seats = _unitOfWork.Seats.GetAll().Where(x => x.HallId == hall.HallId).ToList();
            //            foreach (var seat in seats)
            //            {
            //                _unitOfWork.Seats.Remove(seat);
            //            }
            //            _unitOfWork.Halls.Remove(hall);
            //        }
            //        _unitOfWork.Theatres.Remove(theatre);
            //        _unitOfWork.Save();

            //        return Json(new { success = true });
            //    }
            //    catch (Exception ex)
            //    {
            //        return Json(new { success = false, message = ex.Message });
            //    }
            //}
            //return Json(new { success = false, message = "Theatre is Null!" });

            try
            {
                await _theatreService.DeleteTheatreAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// Converts an IFormFile to FileUploadDto to be used in Application Layer.
        private FileUploadDto ConvertIFormFileToDto(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return new FileUploadDto
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    FileData = memoryStream.ToArray()
                };
            }
        }
    }
}