﻿using ConvenienceMVC.Models.Interfaces.Zaikos;
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
        private IZaikoService zaikoService;

        // コンストラクタ
        public ZaikosController(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
            zaikoService = new ZaikoService(_context);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public IActionResult Search()
        {
            var sokoZaikos = _context.SokoZaiko
                .OrderBy(soko => soko.ShiirePrdId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(shi => shi.ShohinMaster)
                .ToList();

            IList<string?> keyList = new List<string?>();
            IList<bool> flagList = new List<bool>();
            for (int i = 0; i < 3; i++)
            {
                keyList.Add(null);
                flagList.Add(false);
            }

            ZaikoViewModel zaikoViewModel = new ZaikoViewModel()
            {
                KeyEventDataList = keyList,
                DescendingFlagList = flagList,
                SokoZaikos = sokoZaikos,
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

            inZaikoViewModel = zaikoService.SortSokoZaiko(inZaikoViewModel);

            return View("Search", inZaikoViewModel);
        }
    }
}
