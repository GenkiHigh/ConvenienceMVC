using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC.Models.Services.Zaikos;
using ConvenienceMVC.Models.Views.Zaikos;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Controllers
{
    // 在庫コントローラー
    public class ZaikosController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 基底サービスインターフェース
        private readonly IDefineService DefineService;
        // 在庫サービスインターフェース
        private readonly IZaikoService ZaikoService;

        // コンストラクタ
        public ZaikosController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキストを設定
            _context = context;
            // 在庫サービスインスタンス生成
            ZaikoService = new ZaikoService(_context);
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
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Zaikos" });
            }

            return RedirectToAction("Search");
        }

        public IActionResult Search()
        {
            ZaikoSearchViewModel getZaikoSearchViewModel = ZaikoService.ZaikoSetting();

            return View(getZaikoSearchViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ZaikoSearchViewModel inZaikoViewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            }
            ModelState.Clear();

            // ログインしていない場合
            UserLog queriedUserLog = DefineService.IsUserSession();
            if (DefineService.IsUserSession() == null)
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Zaikos" });
            }

            inZaikoViewModel = ZaikoService.DisplayZaikoSetting(inZaikoViewModel);

            return View("Search", inZaikoViewModel);
        }
    }
}
