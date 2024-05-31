using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC.Models.Services.Chumons;
using ConvenienceMVC.Models.Views.Chumons;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConvenienceMVC.Controllers
{
    // 注文コントローラー
    public class ChumonsController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 注文サービス
        private IChumonService ChumonService;

        // コンストラクタ
        public ChumonsController(ConvenienceMVCContext context)
        {
            // DBコンテキスト設定
            _context = context;
            // 注文サービスインスタンス生成
            ChumonService = new ChumonService(_context);
        }

        // Index(初期設定)
        public async Task<IActionResult> Index()
        {
            return View();
        }
        // 総合メニュー移動
        public IActionResult Menu()
        {
            // メニューコントローラーのIndexに移動する
            return RedirectToAction("Index", "Menus");
        }

        // 注文実績検索(初期設定)
        public IActionResult Search()
        {
            // 注文実績検索用ViewModelを設定、Viewに渡す
            var inChumonKeyViewModel = SetChumonKeyViewModel();
            return View(inChumonKeyViewModel);
        }
        /*
         * 注文実績検索(検索実行)
         * inChumonKeyViewModel：選択された仕入先コード、注文日が格納された注文実績検索用ViewModel
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ChumonKeyViewModel inChumonKeyViewModel)
        {
            /*
             * 不具合なく入力されたかを判定
             * 選択された仕入先コード、注文日を基に注文実績を検索し検索結果を受け取る
             * 受け取った注文実績を保存
             * 注文実績を更新画面に渡しながら移動
             */

            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException("Postデータエラー");
            }

            // 注文実績検索(既に注文されたかを問い合わせる)
            ChumonViewModel chumonViewModel = ChumonService.ChumonSetting(
                inChumonKeyViewModel.ShiireSakiId, DateOnly.FromDateTime(inChumonKeyViewModel.ChumonDate));

            // 注文実績保存
            KeepObject();

            // 注文実績明細更新に移動
            return View("Update", chumonViewModel);
        }

        /*
         * 注文実績明細更新(初期画面)
         * inChumonViewModel：検索結果又は新規作成した注文実績を格納した注文実績更新用ViewModel
         */
        public IActionResult Update(ChumonViewModel inChumonViewModel)
        {
            // Viewに渡す
            return View(inChumonViewModel);
        }
        /*
         * 注文実績明細更新(更新実行)
         * id；オーバーロード用引数
         * inChumonViewModel：入力した注文数を格納した注文実績更新用ViewModel
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ChumonViewModel inChumonViewModel)
        {
            /*
             * 不具合なく入力されたかを判定
             * 保存したデータを復元
             * 注文実績を更新し、更新後のデータを受け取る
             * 更新後のデータを保存
             * 注文実績を更新画面に渡しながら移動
             */

            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            ModelState.Clear();

            // 注文実績復元
            GetObject();

            // 注文実績更新
            inChumonViewModel = ChumonService.ChumonCommit(inChumonViewModel);

            // 注文実績保存
            KeepObject();

            // 注文実績明細更新に移動
            return View("Update", inChumonViewModel);
        }

        // 注文実績検索用ViewModel設定
        private ChumonKeyViewModel SetChumonKeyViewModel()
        {
            /*
             * 仕入先コードをまとめたリストを作成
             * 仕入先コードリストを格納したViewModelを作成し戻り値に渡す
             */

            // 仕入先コードリストを設定
            var list = _context.ShiireSakiMaster.OrderBy(s => s.ShiireSakiId).Select(s => new SelectListItem
            {
                Value = s.ShiireSakiId,
                Text = s.ShiireSakiId + " " + s.ShiireSakiKaisya
            }).ToList();
            // 仕入先コードリストを格納したViewModelを作成し渡す
            return new ChumonKeyViewModel()
            {
                ShiireSakiId = null,
                ChumonDate = DateTime.Today,
                ShiireSakiIdList = list
            };
        }

        // オブジェクト保存
        private void KeepObject()
        {
            // シリアライズしてデータ保存
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            TempData["ChumonJisseki"] = JsonConvert.SerializeObject(ChumonService.Chumon.ChumonJisseki, settings);
        }
        // オブジェクト復元
        private void GetObject()
        {
            // デシリアライズしてデータ復元
            string chumonJissekiJson = TempData["ChumonJisseki"] as string;
            ChumonService.Chumon.ChumonJisseki = JsonConvert.DeserializeObject<ChumonJisseki>(chumonJissekiJson);
        }
    }
}
