using CinemaTicketingSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _DbContext;
        public MovieController(ApplicationDbContext DbContext)
        {
            _DbContext = DbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
