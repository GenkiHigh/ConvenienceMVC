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
        [IgnoreAntiforgeryToken]
        public IActionResult Error(int? id)
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            string path = null;
            string error_message = null;

            if (exceptionHandlerPathFeature != null)
            {
                path = exceptionHandlerPathFeature.Path;
                error_message = exceptionHandlerPathFeature.Error.Message;
                _logger.LogError(null, exceptionHandlerPathFeature.Error, "System Error!!");
            }
            else
            {
                _logger.LogError($"Error without exception handler, status code: {id}");
            }

            return View(new ErrorViewModel { Id = id, Path = path, Error_Message = error_message });
        }
    }
}
