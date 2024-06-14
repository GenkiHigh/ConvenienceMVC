using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC.Models.Services.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConvenienceMVC.Controllers
{
    // 仕入コントローラー
    public class ShiiresController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 基底サービスインターフェース
        private readonly IDefineService DefineService;
        // 仕入サービスインターフェース
        private readonly IShiireService ShiireService;

        // コンストラクタ
        public ShiiresController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキストを設定
            _context = context;
            // 仕入サービスのインスタンス生成
            ShiireService = new ShiireService(_context);
            // 基底サービス設定
            DefineService = defineService;
        }

        // Index(初期設定)
        public IActionResult Index()
        {
            // ログインしていない場合
            UserLog queriedUserLog = DefineService.IsUserSession();
            if (DefineService.IsUserSession() == null)
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Shiires" });
            }

            return RedirectToAction("Search");
        }
        // Menu(初期設定)
        public IActionResult Menu()
        {
            // メニューコントローラーのIndexに移動する
            return RedirectToAction("Index", "Menus");
        }

        // 仕入実績、倉庫在庫検索(初期設定)
        public IActionResult Search()
        {
            // 仕入実績、倉庫在庫検索用ViewModelを設定、Viewに渡す
            ShiireSearchViewModel queriedShiireKeyViewModel = SetShiireKeyViewModel();
            return View(queriedShiireKeyViewModel);
        }
        /*
         * 仕入実績、倉庫在庫検索(検索実行)
         * inShiireKeyViewModel：選択された注文コードを格納したViewModel
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ShiireSearchViewModel inShiireKeyViewModel)
        {
            /*
             * 不具合なく入力されたかを判定
             * 選択された注文コードを基に仕入実績、倉庫在庫を検索し検索結果を受け取る
             * 受け取った仕入実績、倉庫在庫を保存
             * 仕入実績、倉庫在庫を更新画面に渡しながら移動
             */

            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };

            // ログインしていない場合
            UserLog queriedUserLog = DefineService.IsUserSession();
            if (DefineService.IsUserSession() == null)
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Shiires" });
            }

            // 仕入実績、倉庫在庫設定
            ShiireUpdateViewModel queriedShiireViewModel = await ShiireService.ShiireSetting(inShiireKeyViewModel.ChumonId);

            // 仕入実績、倉庫在庫保存
            KeepObjects();

            // 仕入実績、倉庫在庫更新にデータを渡しながら移動
            return View("Update", queriedShiireViewModel);
        }

        /*
         * 仕入実績、倉庫在庫更新(初期設定)
         * inShiireViewModel：初期表示したいデータを格納したViewModel
         */
        public IActionResult Update(ShiireUpdateViewModel inShiireViewModel)
        {
            // 更新画面を初期表示
            return View(inShiireViewModel);
        }
        /*
         * 仕入実績、倉庫在庫更新(更新実行)
         * id：オーバーロード用引数
         * inShiireViewModel：入力されたデータを格納したViewModel
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ShiireUpdateViewModel inShiireViewModel)
        {
            /*
             * 不具合なく入力されたかを判定
             * 保存したデータを復元
             * 仕入実績、倉庫在庫を更新し、更新後のデータを受け取る
             * 更新後のデータを保存
             * 仕入実績、倉庫在庫を更新画面に渡しながら移動
             */

            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            ModelState.Clear();

            ShiireUpdateViewModel getShiireViewModel = inShiireViewModel;

            // ログインしていない場合
            UserLog queriedUserLog = DefineService.IsUserSession();
            if (DefineService.IsUserSession() == null)
            {
                // ログインページに移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Shiires" });
            }
            // ログインしている場合
            else
            {
                // 使用中のユーザーIDを設定
                foreach (var shiire in getShiireViewModel.ShiireJissekis)
                {
                    shiire.UserId = queriedUserLog.UserId;
                }
            }

            // データ復元
            GetObjects();

            // 仕入実績、倉庫在庫更新
            ShiireUpdateViewModel queriedShiireViewModel = await ShiireService.ShiireCommit(getShiireViewModel);

            // 仕入実績、倉庫在庫保存
            KeepObjects();

            // 更新画面にデータを渡しながら移動
            return View("Update", queriedShiireViewModel);
        }

        // 仕入実績、倉庫在庫検索用ViewModel設定
        private ShiireSearchViewModel SetShiireKeyViewModel()
        {
            /*
             * 注文コードをまとめたリストを作成
             * 注文残が全て0の注文実績明細の注文コードは表示しない
             * 取得した注文コードリストを降順にソート
             * 注文コードリストを格納したViewModelを作成し戻り値に渡す
             */

            // 全注文コードを格納したリスト(中身変更可能)
            var transList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).Include(chu => chu.ShiireSakiMaster).ToList();
            // 全注文コードを格納したリスト
            var checkList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).ToList();
            // 注文コードに対応する注文実績明細の注文残が全て0のデータを非表示にする
            foreach (ChumonJisseki check in checkList)
            {
                // 注文残が0かを判定
                bool isRemove = true;
                foreach (ChumonJissekiMeisai meisai in check.ChumonJissekiMeisais)
                {
                    if (meisai.ChumonZan != 0) isRemove = false;
                }
                // 注文残が全て0の場合、対象データ削除
                if (isRemove) transList.Remove(check);
            }

            // データ非表示後の注文コードリストの表示内容を設定
            // データ全体を降順にソート(上から注文実績作成日時が最近のデータが並ぶ)
            List<SelectListItem> list = transList.Select(c => new SelectListItem
            {
                Value = c.ChumonId,
                Text = c.ChumonId + " " + c.ShiireSakiMaster.ShiireSakiKaisya,
            }).Reverse().ToList();

            // 注文コードリストを格納したViewModelを作成し渡す
            return new ShiireSearchViewModel()
            {
                ChumonId = null,
                ChumonIds = list,
            };
        }

        // オブジェクト保存
        private void KeepObjects()
        {
            // シリアライズしてデータ保存
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            TempData["ShiireJissekis"] = JsonConvert.SerializeObject(ShiireService.Shiire.ShiireJissekis, settings);
            TempData["SokoZaikos"] = JsonConvert.SerializeObject(ShiireService.Shiire.SokoZaikos, settings);
        }
        // オブジェクト復元
        private void GetObjects()
        {
            // デシリアライズしてデータ復元
            string shiireJissekisJson = TempData["ShiireJissekis"] as string;
            string sokoZaikosJson = TempData["SokoZaikos"] as string;
            ShiireService.Shiire.ShiireJissekis = JsonConvert.DeserializeObject<List<ShiireJisseki>>(shiireJissekisJson);
            ShiireService.Shiire.SokoZaikos = JsonConvert.DeserializeObject<List<SokoZaiko>>(sokoZaikosJson);
        }
    }
}
