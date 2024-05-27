using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Controllers
{
    public class MenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chumon()
        {
            return RedirectToAction("Search", "Chumons");
        }

        public IActionResult Shiire()
        {
            return RedirectToAction("Search", "Shiires");
        }
    }
}
