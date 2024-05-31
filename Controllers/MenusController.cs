using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Controllers
{
    public class MenusController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        public MenusController(ConvenienceMVCContext context)
        {
            _context = context;
        }

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

        // テスト用
        public IActionResult SetShiireMastar()
        {
            var shohinList = _context.ShohinMaster.OrderBy(sho => sho.ShohinId).ToList();
            var shiireSakiList = _context.ShiireSakiMaster.OrderBy(ss => ss.ShiireSakiId).ToList();

            var allShiireMaster = _context.ShiireMaster.ToList();
            _context.ShiireMaster.RemoveRange(allShiireMaster);
            _context.SaveChanges();

            foreach (var shohin in shohinList)
            {
                foreach (var shiireSaki in shiireSakiList)
                {
                    _context.ShiireMaster.Add(new ShiireMaster()
                    {
                        ShiireSakiId = shiireSaki.ShiireSakiId,
                        ShiirePrdId = shohin.ShohinId,
                        ShohinId = shohin.ShohinId,
                        ShiirePrdName = shohin.ShohinName,
                        ShiirePcsPerUnit = 0,
                        ShiireUnit = "個",
                        ShiireTanka = 0,
                    });
                }
            }

            _context.SaveChanges();

            return View("Index");
        }
    }
}
