using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Chumons
{
    public class Chumon : IChumon
    {
        private readonly ConvenienceMVCContext _context;

        public ChumonJisseki ChumonJisseki { get; set; }

        public Chumon(ConvenienceMVCContext context)
        {
            _context = context;
        }

        // 注文作成
        public ChumonJisseki ChumonSakusei(string inShiireSakiCode, DateOnly inChumonDate)
        {
            // 注文ID発番
            string chumonId = ChumonIDCreate(inChumonDate);

            // 注文実績新規作成
            ChumonJisseki = new ChumonJisseki()
            {
                ChumonId = chumonId,
                ShiireSakiId = inShiireSakiCode,
                ChumonDate = inChumonDate,
            };

            // 仕入マスタ取得
            var shiireMastars = _context.ShiireMaster
                .Where(sm => sm.ShiireSakiId == inShiireSakiCode)
                .Include(sm => sm.ShiireSakiMaster)
                .Include(sm => sm.ShohinMaster)
                .OrderBy(sm => sm.ShohinId);

            // 空の注文実績明細作成
            ChumonJisseki.ChumonJissekiMeisais = new List<ChumonJissekiMeisai>();
            foreach (var shiire in shiireMastars)
            {
                ChumonJisseki.ChumonJissekiMeisais.Add(new ChumonJissekiMeisai()
                {
                    ChumonId = chumonId,
                    ShiireSakiId = inShiireSakiCode,
                    ShiirePrdId = shiire.ShiirePrdId,
                    ShohinId = shiire.ShohinId,
                    ChumonSu = 0,
                    ChumonZan = 0,
                    ShiireMaster = shiire,
                });
            }

            return ChumonJisseki;
        }

        // 注文問い合わせ
        public ChumonJisseki ChumonToiawase(string inShiireSakiCode, DateOnly inChumonDate)
        {
            // 注文履歴が残っているかを判断
            ChumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ShiireSakiId == inShiireSakiCode && ch.ChumonDate == inChumonDate)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .FirstOrDefault();

            return ChumonJisseki;
        }

        // 注文更新
        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki)
        {
            // ChumonJissekiは問い合わせor作成の地点で止まってるから
            // inChumonJissekiは入力された注文数を入れたChumonJissekiにすれば比較が出来る
            // 注文数が前回と比較して変更されていれば更新
            // 更新時、注文IDと仕入先IDで検索してあれば更新、なければ追加
            bool changeFlag = false;

            for (int i = 0; i < ChumonJisseki.ChumonJissekiMeisais.Count; i++)
            {
                // 注文数が変更されていたら更新
                if (ChumonJisseki.ChumonJissekiMeisais[i].ChumonSu != inChumonJisseki.ChumonJissekiMeisais[i].ChumonSu)
                {
                    changeFlag = true;
                }
            }

            if (changeFlag)
            {
                //// その注文実績が既にある場合更新、新規の場合追加
                var chumonJisseki = ChumonJisseki;
                chumonJisseki = _context.ChumonJisseki
                    .Where(chu => chu.ChumonId == inChumonJisseki.ChumonId && chu.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                    .Include(chu => chu.ChumonJissekiMeisais)
                    .FirstOrDefault();

                // 更新
                if (chumonJisseki != null)
                {
                    for (int i = 0; i < ChumonJisseki.ChumonJissekiMeisais.Count; i++)
                    {
                        decimal chumonSa = inChumonJisseki.ChumonJissekiMeisais[i].ChumonSu - chumonJisseki.ChumonJissekiMeisais[i].ChumonSu;

                        chumonJisseki.ChumonJissekiMeisais[i].ChumonSu += chumonSa;
                        chumonJisseki.ChumonJissekiMeisais[i].ChumonZan += chumonSa;

                        // 注文残が0未満にならないよう調整
                        if (chumonJisseki.ChumonJissekiMeisais[i].ChumonZan < 0)
                        {
                            chumonJisseki.ChumonJissekiMeisais[i].ChumonSu -= chumonJisseki.ChumonJissekiMeisais[i].ChumonZan;
                            chumonJisseki.ChumonJissekiMeisais[i].ChumonZan = 0;
                        }
                    }

                    ChumonJisseki = chumonJisseki;
                }
                // 追加
                else
                {
                    _context.ChumonJisseki.Add(inChumonJisseki);

                    foreach (var item in inChumonJisseki.ChumonJissekiMeisais)
                    {
                        _context.ChumonJissekiMeisai.Add(item);
                    }

                    ChumonJisseki = inChumonJisseki;
                }
            }

            return ChumonJisseki;
        }

        // 注文ID発番
        public string ChumonIDCreate(DateOnly inChumonDate)
        {
            if (_context.ChumonJisseki.Count() == 0)
            {
                string newDay = inChumonDate.ToString("yyyyMMdd-");
                string newNum = "001";
                return newDay + newNum;
            }

            string chumonId = _context.ChumonJisseki
                .OrderBy(x => x.ChumonId).Select(x => x.ChumonId).Last();
            string chumonDate = inChumonDate.ToString("yyyyMMdd-");
            string getChumonDate = chumonId.Substring(0, 9);
            string num = chumonId.Substring(chumonId.Length - 3);

            // 入力した注文日で初めての注文の場合注文番号を001に、既に注文済みなら番号を1追加
            num = chumonDate == getChumonDate ? (uint.Parse(num) + 1).ToString("D3") : "001";

            return uint.Parse(num) >= 999 ? null : chumonDate + num;
        }
    }
}
