using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC.Models.Services.Zaikos;
using ConvenienceMVC.Models.Views.Zaikos;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Controllers
{
    public class ZaikosController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 基底サービスインターフェース
        private readonly IDefineService DefineService;
        // 在庫サービスインターフェース
        private readonly IZaikoService zaikoService;

        // コンストラクタ
        public ZaikosController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキストを設定
            _context = context;
            // 在庫サービスインスタンス生成
            zaikoService = new ZaikoService(_context);
            // 基底サービス設定
            DefineService = defineService;
        }

        public IActionResult Index()
        {
            // ログインしていない場合
            if (!DefineService.IsUserSession())
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Zaikos" });
            }

            return RedirectToAction("Search");
        }

        public IActionResult Search()
        {
            ZaikoViewModel zaikoViewModel = new ZaikoViewModel();

            // 倉庫在庫設定
            var sokoZaikos = _context.SokoZaiko.AsNoTracking()
                .OrderBy(soko => soko.ShiirePrdId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(shi => shi.ShohinMaster)
                .ToList();

            // ソートキーリスト設定、降順フラグリスト設定
            IList<string?> keyList = new List<string?>();
            IList<bool> flagList = new List<bool>();
            for (int i = 0; i < 3; i++)
            {
                keyList.Add(null);
                flagList.Add(false);
            }

            // 絞り込みリスト設定
            IList<int> numList = new List<int>();
            for (int i = 0; i < zaikoViewModel.TableList.Count; i++)
            {
                int num = zaikoViewModel.SelectCodeList.Where(x => x.Item2 == i).Count();
                numList.Add(num);
            }

            zaikoViewModel = new ZaikoViewModel()
            {
                KeyEventDataList = keyList,
                DescendingFlagList = flagList,
                SokoZaikos = sokoZaikos,
                SetCodesList = new List<string>(),
                LabelNumList = numList,
            };

            return View(zaikoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ZaikoViewModel inZaikoViewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            }
            ModelState.Clear();

            // ログインしていない場合
            if (!DefineService.IsUserSession())
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Zaikos" });
            }

            inZaikoViewModel = zaikoService.SetDisplaySokoZaiko(inZaikoViewModel);

            return View("Search", inZaikoViewModel);
        }
    }
}
