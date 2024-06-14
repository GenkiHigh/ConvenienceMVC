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
            // 基底サービスを設定
            DefineService = defineService;
        }

        // Index(初期設定)
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
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Shiires" });
            }

            // 検索画面に移動
            return RedirectToAction("Search");
        }
        // Menu(初期設定)
        public IActionResult Menu()
        {
            // メニュー画面に移動する
            return RedirectToAction("Index", "Menus");
        }

        // 仕入実績、倉庫在庫検索(初期設定)
        public IActionResult Search()
        {
            // 処理１：仕入実績、倉庫在庫検索用ViewModelを設定する
            // 戻り値：仕入実績、倉庫在庫検索用ViewModelを渡しながら検索画面への移動

            // 処理１：仕入実績、倉庫在庫検索用ViewModelを設定する
            ShiireSearchViewModel getShiireSearchViewModel = SetShiireSearchViewModel();

            // 設定した仕入実績、倉庫在庫検索用ViewModelを渡しながら検索画面に移動する
            return View(getShiireSearchViewModel);
        }
        // 仕入実績、倉庫在庫検索(検索実行)
        // inShiireSearchViewModel：検索画面で入力されたデータを格納した検索用ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ShiireSearchViewModel inShiireSearchViewModel)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：ログイン状況を確認する
            // 処理３：仕入実績、倉庫在庫を検索する
            // 処理４：検索結果、又は作成した仕入実績、倉庫在庫を保存する
            // 戻り値：検索結果、又は作成したデータを格納した更新用ViewModel

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };

            // 処理２：ログイン状況を確認する
            UserLog queriedUserLog = DefineService.IsUserSession();
            // 処理２－１：ログインしていない場合
            if (queriedUserLog == null)
            {
                // ログイン画面に移動
                return RedirectToAction("Index", "UserLogs", new { inPageName = "Shiires" });
            }

            // 処理３：仕入実績、倉庫在庫を検索する
            ShiireUpdateViewModel queriedShiireViewModel = await ShiireService.ShiireSetting(inShiireSearchViewModel.ChumonId);

            // 処理４：検索結果、又は作成した仕入実績、倉庫在庫を保存する
            KeepObjects();

            // 検索結果、又は作成したデータを格納した更新用ViewModelを渡しながら更新画面に移動する
            return View("Update", queriedShiireViewModel);
        }

        // 仕入実績、倉庫在庫更新(初期設定)
        // inShiireUpdateViewModel：検索画面で検索して取得した、又は新規作成したデータを格納した更新用ViewModel
        public IActionResult Update(ShiireUpdateViewModel inShiireUpdateViewModel)
        {
            // 更新用ViewModelを渡しながら更新画面に移動する
            return View(inShiireUpdateViewModel);
        }
        // 仕入実績、倉庫在庫更新(更新実行)
        // id：オーバーロード用
        // inShiireUpdateViewModel：更新画面で入力されたデータを格納した更新用ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ShiireUpdateViewModel inShiireUpdateViewModel)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：ログイン状況を確認する
            // 処理３：仕入実績、倉庫在庫を復元する
            // 処理４：仕入実績、倉庫在庫を更新する
            // 処理５：変更後の仕入実績、倉庫在庫を保存する
            // 戻り値：変更後の更新用ViewModelを渡しながら更新画面への移動

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            // 処理１－１：入力フォームを初期化する
            ModelState.Clear();

            // 処理２：ログイン状況を確認する
            ShiireUpdateViewModel getShiireUpdateViewModel = inShiireUpdateViewModel;
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
                foreach (var shiire in getShiireUpdateViewModel.ShiireJissekis)
                {
                    shiire.UserId = queriedUserLog.UserId;
                }
            }

            // 処理３：仕入実績、倉庫在庫を復元する
            GetObjects();

            // 処理４：仕入実績、倉庫在庫を更新する
            ShiireUpdateViewModel updateShiireViewModel = await ShiireService.ShiireCommit(getShiireUpdateViewModel);

            // 処理５：変更後の仕入実績、倉庫在庫を保存する
            KeepObjects();

            // 変更後のデータを渡しながら更新画面に移動する
            return View("Update", updateShiireViewModel);
        }

        // 仕入実績、倉庫在庫検索用ViewModel設定
        private ShiireSearchViewModel SetShiireSearchViewModel()
        {
            // 処理１：注文残が全て0の仕入実績を削除する
            // 処理２：注文コードリストの表示内容を設定する
            // 戻り値：注文コードリストを格納した検索用ViewModel

            // 処理１：全注文コードを格納したリスト(中身変更可能)を設定する
            var transList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).Include(chu => chu.ShiireSakiMaster).ToList();
            // 処理１－１：全注文コードを格納したリスト(判定用)を設定する
            var checkList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).ToList();
            // 処理１－２：注文コードに対応する注文実績明細の注文残が全て0のデータを非表示にする
            foreach (ChumonJisseki check in checkList)
            {
                // 処理１－２－１：注文残が0かを判定する
                bool isRemove = true;
                foreach (ChumonJissekiMeisai meisai in check.ChumonJissekiMeisais)
                {
                    if (meisai.ChumonZan != 0) isRemove = false;
                }
                // 処理１－２－２：注文残が全て0の場合、対象データを削除する
                if (isRemove) transList.Remove(check);
            }

            // 処理２：データ削除後の注文コードリストの表示内容を設定する
            // データ全体を降順にソートする(上から注文実績作成日時が最近のデータが並ぶ)
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
