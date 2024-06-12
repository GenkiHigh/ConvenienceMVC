using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            // ログイン確認
            if (!CheckUserSession())
            {
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Menus" });
            }

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

        public IActionResult Zaiko()
        {
            return RedirectToAction("Search", "Zaikos");
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
                        ShiirePrdId = shiireSaki.ShiireSakiKaisya + "-" + shohin.ShohinId,
                        ShohinId = shohin.ShohinId,
                        ShiirePrdName = shohin.ShohinName,
                        ShiirePcsPerUnit = SetShiirePcsPerUnit(shohin.ShohinId),
                        ShiireUnit = "個",
                        ShiireTanka = 0,
                    });
                }
            }

            _context.SaveChanges();

            return View("Index");
        }
        // テスト用
        private decimal SetShiirePcsPerUnit(string inShohinId)
        {
            decimal unit = 0;
            if (inShohinId == "SC001")
            {
                unit = 20;
            }
            else if (inShohinId == "SC002")
            {
                unit = 15;
            }
            else if (inShohinId == "SC003")
            {
                unit = 10;
            }
            else if (inShohinId == "SC004")
            {
                unit = 5;
            }
            else
            {
                unit = 0;
            }

            return unit;
        }

        private bool CheckUserSession()
        {
            // セッション情報取得
            var userSession = HttpContext.Session.GetString("MyUserLog");

            // セッション情報が無い場合
            if (userSession == null)
            {
                // ログインページに飛ぶ
                return false;

                UserLog userLogin = new UserLog()
                {
                    UserId = "000000000001",
                    UserName = "テスト太郎",
                    MailAddress = "test@gmail.com",
                    Password = "aaaaaaaa",
                    LastLoginDate = DateTime.Now,
                };

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                HttpContext.Session.SetString("MyInformation", JsonConvert.SerializeObject(userLogin, settings));
            }
            else return true;
        }
    }
}
