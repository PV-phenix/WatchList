using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WatchList.Models;

namespace WatchList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if ((User is not null) && (User.Identity is not null) && (User.Identity.IsAuthenticated))
            {
                return RedirectToAction("Index", "ListeFilms");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}