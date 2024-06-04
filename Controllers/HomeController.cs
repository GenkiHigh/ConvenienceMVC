using ConvenienceMVC.Models.Views;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? id)
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var path = exceptionHandlerPathFeature.Path;
            var error_message = exceptionHandlerPathFeature.Error.Message;

            _logger.LogError(1, exceptionHandlerPathFeature.Error, "System Error!!");

            return View(new ErrorViewModel { Id = id, Path = path, Error_Message = error_message });
        }
    }
}
