using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinemaTicketingSystem.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Get the logged-in user's name from claims
            var userName = User.Identity.IsAuthenticated
                ? User.FindFirstValue(ClaimTypes.Name)
                : "Guest";

            ViewData["UserName"] = userName;
            return View();
        }
    }
}
