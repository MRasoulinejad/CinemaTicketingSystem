using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Repository;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class TheatreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TheatreController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageTheatre()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var theatre = _unitOfWork.Theatres.Get(x=> x.TheatreId == id);
            return View(theatre);
        }

        public IActionResult CreateTheatre()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTheatre(AddTheatreVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.TheatreImage != null)
                {
                    // 📂 define the uploads folder path
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/TheatreImages");

                    // 🛡️ make sure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 📝 make the file name unique
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.TheatreImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 💾 save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.TheatreImage.CopyTo(fileStream);
                    }
                }

                // 🛠️ map the view model to the domain model
                var theatre = new Theatre
                {
                    TheatreName = model.TheatreName,
                    Location = model.Location,
                    TheatreImage = uniqueFileName != null ? "/images/TheatreImages/" + uniqueFileName : null
                };
                
                // 🗃️ Add the movie to the database
                _unitOfWork.Theatres.Add(theatre);
                _unitOfWork.Save();

                return RedirectToAction("ManageTheatre");
            }

            ViewData["Error"] = "Please fill all required fields correctly.";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search([FromBody] string searchTerm)
        {

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Get all movies
                List<Theatre> theatres = _unitOfWork.Theatres.GetAll()
                    .Where(x => x.TheatreName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return PartialView("_TheatreListPartial", theatres);
            }

            return PartialView("_TheatreListPartial", new List<Theatre>());
        }

        public IActionResult UpdateTheatre(int theatreId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateTheatre(Theatre theatre)
        {
            if (ModelState.IsValid)
            {
                // Update the theatre in the database
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTheatre([FromBody]int id)
        {
            
            Theatre theatre = _unitOfWork.Theatres.Get(x => x.TheatreId == id);

            if(theatre != null)
            {
                try
                {
                    List<Hall> halls = _unitOfWork.Halls.GetAll().Where(x => x.TheatreId == theatre.TheatreId).ToList();
                    foreach (var hall in halls)
                    {
                        List<Seat> seats = _unitOfWork.Seats.GetAll().Where(x => x.HallId == hall.HallId).ToList();
                        foreach (var seat in seats)
                        {
                            _unitOfWork.Seats.Remove(seat);
                        }
                        _unitOfWork.Halls.Remove(hall);
                    }
                    _unitOfWork.Theatres.Remove(theatre);
                    _unitOfWork.Save();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                
            }
            return Json(new { success = false, message = "Theatre is Null!" });
        }

        

    }
}