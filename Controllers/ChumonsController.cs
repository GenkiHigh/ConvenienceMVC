using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Services.Chumons;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC.Models.Views.Chumons;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ConvenienceMVC.Controllers
{
    /// <summary>
    /// 注文コントローラー
    /// </summary>
    public class ChumonsController : Controller
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceMVCContext _context;
        /// <summary>
        /// 基底サービスインターフェース
        /// </summary>
        private readonly IDefineService DefineService;
        /// <summary>
        /// 注文サービスインターフェース
        /// </summary>
        private readonly IChumonService ChumonService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="defineService">基底サービス</param>
        public ChumonsController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキスト設定
            _context = context;
            // 注文サービスインスタンス生成
            ChumonService = new ChumonService(_context);
            // 基底サービス設定
            DefineService = defineService;
        }

        /// <summary>
        /// Index(初期設定)
        /// </summary>
        /// <returns>検索画面に移動</returns>
        public IActionResult Index()
        {
            // 処理１：ログイン状況を確認する
            // 戻り値：検索画面、又はログイン画面への移動

            // 処理１：ログイン状況を確認する
            UserLog queriedUserLog = DefineService.IsUserSession();
            // 処理１－１：ログインしていない場合
            if (queriedUserLog == null)
            {
                // ログイン画面に移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Chumons" });
            }

            // 検索画面に移動
            return RedirectToAction("Search");
        }
        /// <summary>
        /// 総合メニュー移動
        /// </summary>
        /// <returns>メニュー画面に移動</returns>
        public IActionResult Menu()
        {
            // メニュー画面に移動する
            return RedirectToAction("Index", "Menus");
        }

        /// <summary>
        /// 注文実績検索(初期設定)
        /// </summary>
        /// <returns>検索画面に移動</returns>
        public IActionResult Search()
        {
            // 処理１：注文実績検索用ViewModelを設定する
            // 戻り値：注文実績検索用ViewModelを渡しながら検索画面への移動

            // 処理１：注文実績検索用ViewModelを設定する
            ChumonSearchViewModel getChumonSearchViewModel = SetChumonSearchViewModel();

            // 設定した注文実績検索用ViewModelを渡しながら検索画面に移動する
            return View(getChumonSearchViewModel);
        }
        /// <summary>
        /// 注文実績検索(検索実行)
        /// </summary>
        /// <param name="inChumonSearchViewModell">検索画面で入力されたデータを格納した注文実績検索用ViewModel</param>
        /// <returns>更新画面に移動</returns>
        /// <exception cref="Exception">Postデータエラー</exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ChumonSearchViewModel inChumonSearchViewModell)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：ログイン状況を確認する
            // 処理３：注文実績を検索する
            // 処理４：検索して取得した、又は新規作成した注文実績を保存する
            // 戻り値：注文実績を検索した時に作成した注文実績更新用ViewModelを渡しながら更新画面への移動

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            }

            // 処理２：ログイン状況を確認する
            UserLog queriedUserLog = DefineService.IsUserSession();
            // 処理２－１：ログインしていない場合
            if (queriedUserLog == null)
            {
                // ログイン画面に移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Chumons" });
            }

            // 処理３：注文実績を検索する
            ChumonUpdateViewModel queriedChumonUpdateViewModel = await ChumonService.ChumonSetting(
                inChumonSearchViewModell.ShiireSakiId, DateOnly.FromDateTime(inChumonSearchViewModell.ChumonDate));

            // 処理４：検索して取得した、又は新規作成した注文実績を保存する
            KeepObject();

            // 注文実績更新用ViewModelを渡しながら更新画面に移動する
            return View("Update", queriedChumonUpdateViewModel);
        }

        /// <summary>
        /// 注文実績更新(初期設定)
        /// </summary>
        /// <param name="inChumonUpdateViewModel">検索画面で検索して取得した、又は新規作成した注文実績を格納した注文実績更新用ViewModel</param>
        /// <returns>更新画面に移動</returns>
        public IActionResult Update(ChumonUpdateViewModel inChumonUpdateViewModel)
        {
            // 注文実績更新用ViewModelを渡しながら更新画面に移動する
            return View(inChumonUpdateViewModel);
        }
        /// <summary>
        /// 注文実績更新(更新実行)
        /// </summary>
        /// <param name="id">オーバーロード用</param>
        /// <param name="inChumonUpdateViewModel">更新画面で入力されたデータを格納した注文実績更新用ViewModel</param>
        /// <returns>再度更新画面に移動</returns>
        /// <exception cref="Exception">Postデータエラー</exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ChumonUpdateViewModel inChumonUpdateViewModel)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：ログイン状況を確認する
            // 処理３：注文実績を復元する
            // 処理４：注文実績を更新する
            // 処理５：変更後の注文実績を保存する
            // 戻り値：変更後の注文実績更新用ViewModelを渡しながら更新画面への移動

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            // 処理１－１：入力フォームを初期化する
            ModelState.Clear();

            // 処理２：ログイン状況を確認する
            ChumonUpdateViewModel getChumonViewModel = inChumonUpdateViewModel;
            UserLog queriedUserLog = DefineService.IsUserSession();
            // 処理２－１：ログインしていない場合
            if (DefineService.IsUserSession() == null)
            {
                // ログイン画面に移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Chumons" });
            }
            // 処理２－２：ログインしている場合
            else
            {
                // 使用中のユーザーIDを設定
                getChumonViewModel.ChumonJisseki.UserId = queriedUserLog.UserId;
            }

            // 処理３：注文実績を復元する
            GetObject();

            // 処理４：注文実績を更新する
            ChumonUpdateViewModel updateChumonViewModel = await ChumonService.ChumonCommit(getChumonViewModel);

            // 処理５：変更後の注文実績を保存する
            KeepObject();

            // 変更後のデータを渡しながら更新画面に移動する
            return View("Update", updateChumonViewModel);
        }

        /// <summary>
        /// 注文実績検索用ViewModel設定
        /// </summary>
        /// <returns>更新用ViewModel</returns>
        private ChumonSearchViewModel SetChumonSearchViewModel()
        {
            // 処理１：仕入先コードリストを設定する
            // 戻り値：仕入先コードリストを格納した注文実績検索用ViewModelを作成し渡す

            // 処理１：仕入先コードリストを設定する
            var queriedShiireSakiIdList = _context.ShiireSakiMaster
                .OrderBy(s => s.ShiireSakiId)
                .Select(s => new SelectListItem
                {
                    Value = s.ShiireSakiId,
                    Text = s.ShiireSakiId + " " + s.ShiireSakiKaisya
                })
                .ToList();
            // 仕入先コードリストを格納した注文実績検索用ViewModelを作成し渡す
            return new ChumonSearchViewModel()
            {
                ShiireSakiId = null,
                ChumonDate = DateTime.Today,
                ShiireSakiIdList = queriedShiireSakiIdList,
            };
        }

        /// <summary>
        /// オブジェクト保存
        /// </summary>
        private void KeepObject()
        {
            // シリアライズしてデータ保存
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            TempData["ChumonJisseki"] = JsonConvert.SerializeObject(ChumonService.Chumon.ChumonJisseki, settings);
        }
        /// <summary>
        /// オブジェクト復元
        /// </summary>
        private void GetObject()
        {
            // デシリアライズしてデータ復元
            string chumonJissekiJson = TempData["ChumonJisseki"] as string;
            ChumonService.Chumon.ChumonJisseki = JsonConvert.DeserializeObject<ChumonJisseki>(chumonJissekiJson);
        }
    }
}
