using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
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

        // 仕入サービスインターフェース
        private IShiireService ShiireService;

        // コンストラクタ
        public ShiiresController(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
            // 仕入サービスのインスタンス生成
            ShiireService = new ShiireService(_context);
        }

        // Index(初期設定)
        public IActionResult Index()
        {
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
            ShiireKeyViewModel queriedShiireKeyViewModel = SetShiireKeyViewModel();
            return View(queriedShiireKeyViewModel);
        }
        /*
         * 仕入実績、倉庫在庫検索(検索実行)
         * inShiireKeyViewModel：選択された注文コードを格納したViewModel
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ShiireKeyViewModel inShiireKeyViewModel)
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

            // 仕入実績、倉庫在庫設定
            ShiireViewModel queriedShiireViewModel = ShiireService.ShiireSetting(inShiireKeyViewModel.ChumonId);

            // 仕入実績、倉庫在庫保存
            KeepObjects();

            // 仕入実績、倉庫在庫更新にデータを渡しながら移動
            return View("Update", queriedShiireViewModel);
        }

        /*
         * 仕入実績、倉庫在庫更新(初期設定)
         * inShiireViewModel：初期表示したいデータを格納したViewModel
         */
        public IActionResult Update(ShiireViewModel inShiireViewModel)
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
        public async Task<IActionResult> Update(int id, ShiireViewModel inShiireViewModel)
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

            // データ復元
            GetObjects();

            // 仕入実績、倉庫在庫更新
            ShiireViewModel queriedShiireViewModel = await ShiireService.ShiireCommit(inShiireViewModel);

            // 仕入実績、倉庫在庫保存
            KeepObjects();

            // 更新画面にデータを渡しながら移動
            return View("Update", queriedShiireViewModel);
        }

        // 仕入実績、倉庫在庫検索用ViewModel設定
        private ShiireKeyViewModel SetShiireKeyViewModel()
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
            foreach (var item in checkList)
            {
                // 注文残が0かを判定
                bool removeFlag = true;
                foreach (var chu in item.ChumonJissekiMeisais)
                {
                    if (chu.ChumonZan != 0) removeFlag = false;
                }
                // 注文残が全て0の場合、対象データ削除
                if (removeFlag) transList.Remove(item);
            }

            // データ非表示後の注文コードリストの表示内容を設定
            // データ全体を降順にソート(上から注文実績作成日時が最近のデータが並ぶ)
            var list = transList.Select(c => new SelectListItem
            {
                Value = c.ChumonId,
                Text = c.ChumonId + " " + c.ShiireSakiMaster.ShiireSakiKaisya,
            }).Reverse().ToList();

            // 注文コードリストを格納したViewModelを作成し渡す
            return new ShiireKeyViewModel()
            {
                ChumonId = null,
                ChumonIds = list,
            };
        }

        // オブジェクト保存
        private void KeepObjects()
        {
            // シリアライズしてデータ保存
            var settings = new JsonSerializerSettings
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
