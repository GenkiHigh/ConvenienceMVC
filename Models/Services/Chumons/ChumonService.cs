using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC.Models.Properties.Chumons;
using ConvenienceMVC.Models.Views.Chumons;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Chumons
{
    // 中間
    // ControllerとModelの間を取り持つ
    public class ChumonService : IChumonService
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 注文インターフェース
        public IChumon Chumon { get; set; }

        // コンストラクタ
        public ChumonService(ConvenienceMVCContext context)
        {
            _context = context;
            Chumon = new Chumon(_context);
        }

        // 注文セッティング
        public ChumonViewModel ChumonSetting(string inShiireSakiId, DateOnly inChumonDate)
        {
            ChumonJisseki chumonJisseki;
            chumonJisseki = Chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

            // 注文実績明細検索
            // 指定した注文実績明細が存在している場合
            if (chumonJisseki != default)
            {
                // 要素インクルード
                chumonJisseki = IncludeElements(chumonJisseki);
            }
            else
            {
                // 新規データ作成
                chumonJisseki = Chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
            }

            // 注文実績更新
            Chumon.ChumonJisseki = chumonJisseki;

            return new ChumonViewModel()
            {
                ChumonJisseki = chumonJisseki,
                IsNormal = false,
                Remark = string.Empty
            };
        }

        // 注文決定
        public ChumonViewModel ChumonCommit(ChumonViewModel inChumonViewModel)
        {
            // 注文数が変更されたかを判定
            bool changeFlag = false;
            for (int i = 0; i < inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais.Count; i++)
            {
                // 変更前の注文数と変更後の注文数を比較して変化しているかを判定
                if (Chumon.ChumonJisseki.ChumonJissekiMeisais[i].ChumonSu != inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[i].ChumonSu)
                {
                    changeFlag = true;
                }
                // 変更後の注文数残を同期
                inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[i].ChumonZan = inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[i].ChumonSu;
            }
            // 注文更新
            // 前回か最新の結果が返ってくる
            Chumon.ChumonJisseki = Chumon.ChumonUpdate(inChumonViewModel.ChumonJisseki);
            // DB更新
            _context.SaveChanges();
            // 要素インクルード
            inChumonViewModel.ChumonJisseki = IncludeElements(inChumonViewModel.ChumonJisseki);

            // 注文実績更新
            Chumon.ChumonJisseki = inChumonViewModel.ChumonJisseki;

            return new ChumonViewModel()
            {
                ChumonJisseki = inChumonViewModel.ChumonJisseki,
                IsNormal = changeFlag ? true : false,
                Remark = changeFlag ? "更新完了" : string.Empty,
            };
        }

        // 要素インクルード
        public ChumonJisseki IncludeElements(ChumonJisseki inChumonJisseki)
        {
            inChumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ChumonId == inChumonJisseki.ChumonId && ch.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .FirstOrDefault();
            inChumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ChumonId == inChumonJisseki.ChumonId && ch.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(shs => shs.ShiireSakiMaster)
                .ThenInclude(shi => shi.ShiireMasters)
                .ThenInclude(sho => sho.ShohinMaster)
                .FirstOrDefault();

            return inChumonJisseki;
        }
    }
}
