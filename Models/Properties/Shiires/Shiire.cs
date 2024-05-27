using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Shiires
{
    public class Shiire : IShiire
    {
        private readonly ConvenienceMVCContext _context;

        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        public IList<SokoZaiko> SokoZaikos { get; set; }

        public Shiire(ConvenienceMVCContext context)
        {
            _context = context;
        }

        public IList<ShiireJisseki> ShiireToiawase(string inChumonId)
        {
            ShiireJissekis = _context.ShiireJisseki
                .Where(sj => sj.ChumonId == inChumonId)
                .Include(sj => sj.ChumonJissekiMeisai)
                .ThenInclude(mei => mei.ChumonJisseki)
                .ThenInclude(chu => chu.ShiireSakiMaster)
                .ThenInclude(ss => ss.ShiireMasters)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToList();

            return ShiireJissekis;
        }

        public IList<ShiireJisseki> ShiireSakusei(string inChumonId)
        {
            // 仕入日時
            DateTime shiireDateTime = DateTime.Now;
            // 仕入日付
            DateOnly shiireDate = DateOnly.FromDateTime(shiireDateTime);
            // 仕入SEQ
            uint seqByShiireDate;
            // 仕入先コード
            string shiireSakiId;
            // 仕入商品コード
            List<string> shiirePrdIds;
            // 商品コード
            List<string> shohinIds;
            // 納入数
            decimal nonyuSu = 0;

            // 仕入SEQ設定
            seqByShiireDate = _context.ShiireJisseki.Count() == 0 ? 1 :
                _context.ShiireJisseki.Select(sj => sj.SeqByShiireDate).Max() + 1;

            // 特定の注文実績明細取得
            var meisai = _context.ChumonJissekiMeisai.Where(mei => mei.ChumonId == inChumonId);

            // 仕入先コード設定
            shiireSakiId = meisai.Select(mei => mei.ShiireSakiId).FirstOrDefault();
            //仕入商品コード設定
            shiirePrdIds = meisai.Select(mei => mei.ShiirePrdId).ToList();
            // 商品コード設定
            shohinIds = meisai.Select(mei => mei.ShohinId).ToList();

            // 空の仕入実績追加
            for (int i = 0; i < meisai.Count(); i++)
            {
                ShiireJissekis.Add(new ShiireJisseki()
                {
                    ChumonId = inChumonId,
                    ShiireDate = shiireDate,
                    SeqByShiireDate = seqByShiireDate,
                    ShiireDateTime = shiireDateTime,
                    ShiireSakiId = shiireSakiId,
                    ShiirePrdId = shiirePrdIds[i],
                    ShohinId = shohinIds[i],
                    NonyuSu = nonyuSu,
                });
                ShiireJissekis[i].ChumonJissekiMeisai = _context.ChumonJissekiMeisai
                    .Where(mei => mei.ChumonId == inChumonId && mei.ShiireSakiId == shiireSakiId &&
                    mei.ShiirePrdId == shiirePrdIds[i] && mei.ShohinId == shohinIds[i])
                    .Include(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(sak => sak.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();
            }

            return ShiireJissekis;
        }

        public bool ChumonJissekiMeisaiToiawase(string inChumonId)
        {
            if (_context.ChumonJissekiMeisai.Count() == 0) return false;

            var check = _context.ChumonJissekiMeisai.Where(mei => mei.ChumonId == inChumonId).FirstOrDefault();

            // 注文はされている場合true
            if (check != null) return true;
            // 注文もされていない場合false
            else return false;
        }

        public IList<SokoZaiko> ZaikoToiawase(string inShiireSakiId)
        {
            SokoZaikos = _context.SokoZaiko
                .Where(soko => soko.ShiireSakiId == inShiireSakiId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToList();

            return SokoZaikos;
        }

        // 在庫作成
        public IList<SokoZaiko> ZaikoSakusei(string inShiireSakiId)
        {
            // 仕入マスタを設定
            var shiireMastars = _context.ShiireMaster
                .Where(sm => sm.ShiireSakiId == inShiireSakiId)
                .ToList();

            foreach (var shiire in shiireMastars)
            {
                SokoZaiko soko = new SokoZaiko()
                {
                    ShiireSakiId = inShiireSakiId,
                    ShiirePrdId = shiire.ShiirePrdId,
                    ShohinId = shiire.ShohinId,
                    SokoZaikoCaseSu = 0,
                    SokoZaikoSu = 0,
                    LastShiireDate = DateOnly.FromDateTime(DateTime.Today),
                    LastDeliveryDate = null,
                };

                SokoZaikos.Add(soko);
            }

            return SokoZaikos;
        }

        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis)
        {
            // DBに対象の仕入実績があったら更新
            // 無かったら追加

            // 前回と比べて納入数が変化しているかを調査
            bool changeFlag = false;
            for (int i = 0; i < inShiireJissekis.Count; i++)
            {
                // 納入数が違う場合
                if (ShiireJissekis[i].NonyuSu != inShiireJissekis[i].NonyuSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            if (changeFlag)
            {
                IList<ShiireJisseki> shiireJissekis = ShiireJissekis;
                shiireJissekis = _context.ShiireJisseki
                    .Where(sj => sj.ShiireSakiId == inShiireJissekis.First().ShiireSakiId)
                    .OrderBy(sj => sj.ShohinId)
                    .ToList();

                // DBに無い場合追加
                if (shiireJissekis.Count != 0)
                {
                    for (int i = 0; i < shiireJissekis.Count; i++)
                    {
                        shiireJissekis[i].NonyuSu = inShiireJissekis[i].NonyuSu;
                    }

                    ShiireJissekis = shiireJissekis;
                }
                else
                {
                    for (int i = 0; i < inShiireJissekis.Count; i++)
                    {
                        inShiireJissekis[i].ShiireDateTime = DateTime.SpecifyKind(inShiireJissekis[i].ShiireDateTime, DateTimeKind.Utc);
                        _context.ShiireJisseki.Add(inShiireJissekis[i]);
                    }

                    // 仕入実績履歴を更新
                    ShiireJissekis = inShiireJissekis;
                }
            }

            return ShiireJissekis;
        }

        public IList<SokoZaiko> ZaikoUpdate(IList<SokoZaiko> inSokoZaikos)
        {
            // DBに対象の倉庫在庫があったら更新
            // 無かったら追加

            // 前回と比べて在庫数が変化しているかを調査
            bool changeFlag = false;
            for (int i = 0; i < inSokoZaikos.Count; i++)
            {
                // 在庫数が違う場合
                if (SokoZaikos[i].SokoZaikoSu != inSokoZaikos[i].SokoZaikoSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            if (changeFlag)
            {
                IList<SokoZaiko> sokoZaikos = SokoZaikos;
                sokoZaikos = _context.SokoZaiko
                    .Where(sz => sz.ShiireSakiId == inSokoZaikos.First().ShiireSakiId)
                    .OrderBy(sz => sz.ShohinId)
                    .ToList();

                // 更新
                if (sokoZaikos.Count != 0)
                {
                    for (int i = 0; i < sokoZaikos.Count; i++)
                    {
                        sokoZaikos[i].SokoZaikoSu = inSokoZaikos[i].SokoZaikoSu;
                    }

                    SokoZaikos = sokoZaikos;
                }
                // 追加
                else
                {
                    for (int i = 0; i < inSokoZaikos.Count; i++)
                    {
                        _context.SokoZaiko.Add(inSokoZaikos[i]);
                    }

                    // 倉庫在庫履歴を更新
                    SokoZaikos = inSokoZaikos;
                }
            }

            return SokoZaikos;
        }

        public ShiireViewModel ChumonZanBalance(ShiireViewModel inShiireViewModel)
        {
            // 注文実績明細取得
            var meisai = _context.ChumonJissekiMeisai
                .Where(mei => mei.ShiireSakiId == inShiireViewModel.ShiireJissekis.First().ShiireSakiId)
                .OrderBy(mei => mei.ShohinId)
                .ToList();

            for (int i = 0; i < inShiireViewModel.ShiireJissekis.Count; i++)
            {
                // 注文残以上の納入不可
                decimal nonyuSu = inShiireViewModel.ShiireJissekis[i].NonyuSu - ShiireJissekis[i].NonyuSu <= meisai[i].ChumonZan ?
                    inShiireViewModel.ShiireJissekis[i].NonyuSu : ShiireJissekis[i].NonyuSu + meisai[i].ChumonZan;

                // 前回の納入数と比較して注文残、在庫数の変動量を設定
                decimal transNum = nonyuSu - ShiireJissekis[i].NonyuSu;

                // 注文残減少
                meisai[i].ChumonZan -= transNum;
                // 在庫数増加
                inShiireViewModel.SokoZaikos[i].SokoZaikoSu += transNum;
                // 納入数同期
                inShiireViewModel.ShiireJissekis[i].NonyuSu = nonyuSu;
            }

            return new ShiireViewModel()
            {
                ShiireJissekis = inShiireViewModel.ShiireJissekis,
                SokoZaikos = inShiireViewModel.SokoZaikos,
            };
        }
    }
}
