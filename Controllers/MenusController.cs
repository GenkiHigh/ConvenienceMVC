using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Controllers
{
    public class MenusController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 基底サービス
        private readonly IDefineService DefineService;

        // コンストラクタ
        public MenusController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキスト設定
            _context = context;
            // 基底サービス設定
            DefineService = defineService;
        }

        public IActionResult Index()
        {
            // ログインしていない場合
            UserLog queriedUserLog = DefineService.IsUserSession();
            if (DefineService.IsUserSession() == null)
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Menus" });
            }

            return View();
        }

        public IActionResult Chumon()
        {
            return RedirectToAction("Index", "Chumons");
        }

        public IActionResult Shiire()
        {
            return RedirectToAction("Index", "Shiires");
        }

        public IActionResult Zaiko()
        {
            return RedirectToAction("Index", "Zaikos");
        }

        // テスト用
        // 仕入マスタを手っ取り早く設定する
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
    }
}
