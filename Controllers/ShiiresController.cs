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
    public class ShiiresController : Controller
    {
        private readonly ConvenienceMVCContext _context;
        private IShiireService ShiireService;

        public ShiiresController(ConvenienceMVCContext context)
        {
            _context = context;
            ShiireService = new ShiireService(_context);
        }

        public IActionResult Index()
        {
            return View();
        }

        // 仕入実績検索(初期設定)
        public IActionResult Search()
        {
            var shiireKeyViewModel = SetShiireKeyViewModel();

            return View(shiireKeyViewModel);
        }
        // 仕入実績検索(検索実行)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ShiireKeyViewModel inShiireKeyViewModel)
        {
            // 1.入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };

            // 2.仕入実績検索
            ShiireViewModel shiireViewModel = ShiireService.ShiireSetting(inShiireKeyViewModel.ChumonId);

            // 3.仕入実績保存
            KeepObjects();

            // 仕入実績更新に移動
            return View("Update", shiireViewModel);
        }

        // 仕入実績更新(初期設定)
        public IActionResult Update(ShiireViewModel inShiireViewModel)
        {
            return View(inShiireViewModel);
        }
        // 仕入実績更新(更新実行)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ShiireViewModel inShiireViewModel)
        {
            // 1.入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            ModelState.Clear();

            // 2.仕入実績内容決定
            GetObjects();
            inShiireViewModel = ShiireService.ShiireCommit(inShiireViewModel);

            // 3.仕入実績保存
            KeepObjects();

            // 仕入実績更新に移動
            return View("Update", inShiireViewModel);
        }

        private ShiireKeyViewModel SetShiireKeyViewModel()
        {
            // 注文残が全て0の注文実績は表示しない
            var transList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).Include(chu => chu.ShiireSakiMaster).ToList();
            var chexkList = _context.ChumonJisseki.Include(chu => chu.ChumonJissekiMeisais).ToList();
            foreach (var item in chexkList)
            {
                bool removeFlag = true;
                foreach (var chu in item.ChumonJissekiMeisais)
                {
                    if (chu.ChumonZan != 0) removeFlag = false;
                }
                if (removeFlag) transList.Remove(item);
            }
            var list = transList.OrderBy(c => c.ChumonId).Select(c => new SelectListItem
            {
                Value = c.ChumonId,
                Text = c.ChumonId + " " + c.ShiireSakiMaster.ShiireSakiKaisya,
            }).ToList();
            return new ShiireKeyViewModel()
            {
                ChumonId = null,
                ChumonIdList = list,
            };
        }

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
