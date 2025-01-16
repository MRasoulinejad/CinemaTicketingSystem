using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Web.Models;
using CinemaTicketingSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            // Take 3 Random Theatres
            var theatres = _unitOfWork.Theatres.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();

            // Take 9 Random Movies
            var movies = _unitOfWork.Movies.GetAll()
                .OrderBy(x => Guid.NewGuid())
                .Take(9)
                .ToList();


            // Pass both to the View using a ViewModel
            var HomeVM = new HomeViewModel
            {
                RandomTheatres = theatres,
                LatestMovies = movies
            };

            ViewData["HeroImageUrl"] = "/images/hero-banner.jpg";
            ViewData["HeroTitle"] = "Experience Movies Like Never Before!";
            ViewData["HeroSubtitle"] = "Find your favorite movies and book tickets now.";
            return View(HomeVM);
        }

        public IActionResult Privacy()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["HeroImageUrl"] = "/images/about-hero.jpg";
            ViewData["HeroTitle"] = "About Our Cinema";
            ViewData["HeroSubtitle"] = "Learn more about our state-of-the-art facilities.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
