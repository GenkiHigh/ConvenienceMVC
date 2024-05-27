using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Properties.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Shiires
{
    public class ShiireService : IShiireService
    {
        private readonly ConvenienceMVCContext _context;
        public IShiire Shiire { get; set; }

        // コンストラクタ
        public ShiireService(ConvenienceMVCContext context)
        {
            _context = context;
            Shiire = new Shiire(_context);
        }

        // 仕入実績設定
        public ShiireViewModel ShiireSetting(string inChumonId)
        {
            // 仕入実績を検索
            Shiire.ShiireJissekis = Shiire.ShiireToiawase(inChumonId);
            // 仕入実績を検索して見つからなかった場合
            if (Shiire.ShiireJissekis.Count == 0)
            {
                // 注文実績明細があるかどうかを調べる
                // 注文はされているけどまだ仕入れていない可能性もある為
                // 注文はされている場合
                if (Shiire.ChumonJissekiMeisaiToiawase(inChumonId))
                {
                    // 仕入実績新規作成
                    Shiire.ShiireJissekis = Shiire.ShiireSakusei(inChumonId);
                }
                // 注文すらされていない場合
                else
                {
                    // エラー
                    // 「注文されていません」
                    throw new Exception("注文されていません");
                }
            }

            // 倉庫在庫を検索
            Shiire.SokoZaikos = Shiire.ZaikoToiawase(Shiire.ShiireJissekis.FirstOrDefault().ShiireSakiId);
            // 倉庫在庫を検索して見つからなかった場合
            if (Shiire.SokoZaikos.Count == 0)
            {
                // 倉庫在庫新規作成
                Shiire.SokoZaikos = Shiire.ZaikoSakusei(Shiire.ShiireJissekis.FirstOrDefault().ShiireSakiId);
            }

            return new ShiireViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                IsNormal = false,
                Remark = string.Empty,
            };
        }

        // 仕入実績内容決定
        public ShiireViewModel ShiireCommit(ShiireViewModel inShiireViewModel)
        {
            // 内容が更新されたか判断
            bool changeFlag = false;
            for (int i = 0; i < inShiireViewModel.ShiireJissekis.Count; i++)
            {
                if (Shiire.ShiireJissekis[i].NonyuSu != inShiireViewModel.ShiireJissekis[i].NonyuSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            // 納入数に応じて注文数減少、在庫数増加
            inShiireViewModel = Shiire.ChumonZanBalance(inShiireViewModel);
            // 仕入実績更新
            inShiireViewModel.ShiireJissekis = Shiire.ShiireUpdate(inShiireViewModel.ShiireJissekis);
            // 在庫倉庫更新
            inShiireViewModel.SokoZaikos = Shiire.ZaikoUpdate(inShiireViewModel.SokoZaikos);
            // DB更新
            _context.SaveChanges();

            // 仕入実績必要要素インクルード
            inShiireViewModel.ShiireJissekis = IncludeShiireJissekis(inShiireViewModel.ShiireJissekis);
            // 倉庫在庫必要要素インクルード
            inShiireViewModel.SokoZaikos = IncludeSokoZaikos(inShiireViewModel.SokoZaikos);

            // 仕入実績更新
            Shiire.ShiireJissekis = inShiireViewModel.ShiireJissekis;
            // 倉庫在庫更新
            Shiire.SokoZaikos = inShiireViewModel.SokoZaikos;

            return new ShiireViewModel()
            {
                ShiireJissekis = inShiireViewModel.ShiireJissekis,
                SokoZaikos = inShiireViewModel.SokoZaikos,
                IsNormal = changeFlag ? true : false,
                Remark = changeFlag ? "更新完了" : string.Empty,
            };
        }

        // 仕入実績必要要素インクルード
        private IList<ShiireJisseki>? IncludeShiireJissekis(IList<ShiireJisseki>? inShiireJissekis)
        {
            List<ShiireJisseki> shiireJissekis = new List<ShiireJisseki>();
            foreach (var shiire in inShiireJissekis)
            {
                ShiireJisseki shiireJisseki = _context.ShiireJisseki
                    .Where(sj => sj.ChumonId == shiire.ChumonId && sj.ShiireDate == shiire.ShiireDate &&
                    sj.SeqByShiireDate == shiire.SeqByShiireDate && sj.ShiireSakiId == shiire.ShiireSakiId &&
                    sj.ShiirePrdId == shiire.ShiirePrdId)
                    .Include(sj => sj.ChumonJissekiMeisai)
                    .ThenInclude(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(saki => saki.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();

                shiireJissekis.Add(shiireJisseki);
            }

            return shiireJissekis;
        }
        // 倉庫在庫必要要素インクルード
        private IList<SokoZaiko>? IncludeSokoZaikos(IList<SokoZaiko>? inSokoZaikos)
        {
            List<SokoZaiko> sokoZaikos = new List<SokoZaiko>();
            foreach (var zaiko in inSokoZaikos)
            {
                SokoZaiko sokoZaiko = _context.SokoZaiko
                    .Where(sk => sk.ShiireSakiId == zaiko.ShiireSakiId && sk.ShiirePrdId == zaiko.ShiirePrdId &&
                    sk.ShohinId == zaiko.ShohinId)
                    .Include(sk => sk.ShiireMaster)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();

                sokoZaikos.Add(sokoZaiko);
            }

            return sokoZaikos;
        }
    }
}
