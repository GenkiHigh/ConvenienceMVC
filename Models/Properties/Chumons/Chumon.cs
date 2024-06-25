using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Chumons
{
    // 注文プロパティ
    public class Chumon : IChumon
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 注文実績
        public ChumonJisseki ChumonJisseki { get; set; }

        // コンストラクタ
        public Chumon(ConvenienceMVCContext context)
        {
            // DBコンテキスト設定
            _context = context;
        }

        // 注文実績検索
        // inShiireSakiCode：検索画面で選択された仕入先コード
        // inChumonDate：検索画面で選択された仕入先コード
        public async Task<ChumonJisseki> ChumonQuery(string inShiireSakiCode, DateOnly inChumonDate)
        {
            // 機能１：注文実績を検索する
            // 戻り値：見つかった注文実績、又はnull

            // 機能１：選択された仕入先コードで注文実績を検索する
            ChumonJisseki? isChumonJisseki = await _context.ChumonJisseki
                .Where(ch => ch.ShiireSakiId == inShiireSakiCode && ch.ChumonDate == inChumonDate)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .Include(ch => ch.ShiireSakiMaster)
                .FirstOrDefaultAsync();

            // 見つけた注文実績、又はnullを格納する
            ChumonJisseki = isChumonJisseki;

            // 見つけた注文実績、又はnullを渡す
            return ChumonJisseki;
        }

        // 注文実績作成
        // inShiireSakiCode：検索画面で選択された仕入先コード
        // inChumonDate：検索画面で選択された仕入先コード
        public async Task<ChumonJisseki> ChumonCreate(string inShiireSakiCode, DateOnly inChumonDate)
        {
            // 機能１：新規注文実績を作成する
            // 機能２：新規注文実績明細を作成する
            // 戻り値：新規作成した注文実績、注文実績明細

            // 機能１：注文IDを発番する
            string createdChumonId = await ChumonIDCreate(inChumonDate);
            // 機能１ー１：新規注文実績を作成する
            ChumonJisseki newChumonJisseki = new ChumonJisseki()
            {
                ChumonId = createdChumonId,
                ShiireSakiId = inShiireSakiCode,
                ChumonDate = inChumonDate,
            };

            // 機能２：仕入マスタを取得する
            IOrderedQueryable<ShiireMaster> queriedShiireMastars = _context.ShiireMaster.AsNoTracking()
                .Where(sm => sm.ShiireSakiId == inShiireSakiCode)
                .Include(sm => sm.ShiireSakiMaster)
                .Include(sm => sm.ShohinMaster)
                .OrderBy(sm => sm.ShohinId);
            // 機能２ー１：新規注文実績明細を作成する
            IList<ChumonJissekiMeisai> newChumonJissekiMeisais = new List<ChumonJissekiMeisai>();
            foreach (ShiireMaster shiire in queriedShiireMastars)
            {
                newChumonJissekiMeisais.Add(new ChumonJissekiMeisai()
                {
                    ChumonId = createdChumonId,
                    ShiireSakiId = inShiireSakiCode,
                    ShiirePrdId = shiire.ShiirePrdId,
                    ShohinId = shiire.ShohinId,
                    ChumonSu = 0,
                    ChumonZan = 0,
                    ShiireMaster = shiire,
                });
            }

            // 注文実績を設定する
            newChumonJisseki.ChumonJissekiMeisais = newChumonJissekiMeisais;
            ChumonJisseki = newChumonJisseki;

            // 新規作成した注文実績及び明細を渡す
            return ChumonJisseki;
        }

        // 注文実績更新
        // inChumonJisseki：更新画面で入力された注文実績
        public async Task<ChumonJisseki> ChumonUpdate(ChumonJisseki inChumonJisseki)
        {
            // 機能１：注文実績を検索する
            // 機能２A：注文実績がある場合、注文数残を変動させる
            // 機能２B：注文実績が無い場合、注文実績、注文実績明細を追加する
            // 機能３A：注文実績がある場合、注文実績を更新する
            // 戻り値：追加、又は更新後の注文実績

            // 機能１：注文実績を検索する
            ChumonJisseki? isChumonJisseki = await _context.ChumonJisseki
                .Where(chu => chu.ChumonId == inChumonJisseki.ChumonId && chu.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(chu => chu.ChumonJissekiMeisais)
                .FirstOrDefaultAsync();
            // 機能１ー１：DBに注文実績がある場合
            if (isChumonJisseki != null)
            {
                // 楽観的排他制御
                var current = _context.Entry(inChumonJisseki).Property(v => v.Version).CurrentValue;
                var current2 = _context.Entry(isChumonJisseki).Property(v => v.Version).CurrentValue;
                if (current != current2)
                {
                    throw new Exception("既に別のデータが更新されています");
                }
                // 注文者が前回と違う場合アベンド
                if (isChumonJisseki.UserId != inChumonJisseki.UserId)
                {
                    throw new Exception("注文者が違います");
                }

                // 機能２A：注文数残を変動させる
                ChumonJisseki queriedChumonJisseki = isChumonJisseki;
                for (int meisaisCounter = 0; meisaisCounter < queriedChumonJisseki.ChumonJissekiMeisais.Count; meisaisCounter++)
                {
                    // 機能２A－１：入力前と入力後の注文数の差分、注文数残を変動させる
                    decimal chumonSa = inChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu -
                        queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu;
                    queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu += chumonSa;
                    queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan += chumonSa;
                    // 機能２A－２；注文残が0未満にならないよう調整する
                    if (queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan < 0)
                    {
                        queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu -=
                            queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan;
                        queriedChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan = 0;
                    }
                }

                // 機能３A：注文実績、注文実績明細を更新する
                _context.ChumonJisseki.Update(isChumonJisseki);
                foreach (ChumonJissekiMeisai meisai in queriedChumonJisseki.ChumonJissekiMeisais)
                {
                    _context.ChumonJissekiMeisai.Update(meisai);
                }

                // 注文実績を設定する
                ChumonJisseki = isChumonJisseki;
            }
            // 機能１－２：DBに注文実績が無い場合
            else
            {
                bool isAdd = false;
                for (int meisaisCounter = 0; meisaisCounter < inChumonJisseki.ChumonJissekiMeisais.Count; meisaisCounter++)
                {
                    if (inChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu != 0)
                    {
                        isAdd = true;
                        break;
                    }
                }
                if (isAdd)
                {
                    // 機能２B：注文実績、注文実績明細を追加する
                    _context.ChumonJisseki.Add(inChumonJisseki);
                    foreach (ChumonJissekiMeisai meisai in inChumonJisseki.ChumonJissekiMeisais)
                    {
                        _context.ChumonJissekiMeisai.Add(meisai);
                    }
                }

                // 注文実績を設定する
                ChumonJisseki = inChumonJisseki;
            }

            // 更新後の注文実績を渡す
            return ChumonJisseki;
        }

        // 注文ID発番
        // inChumonDate：検索画面で選択された注文日
        private async Task<string> ChumonIDCreate(DateOnly inChumonDate)
        {
            // 機能１：注文ID、選択された注文日、DB内の注文日、注文番号を設定する
            // 戻り値："注文年月日-注文番号三桁"の文字列(その日の注文実績数が1000以上の場合null)

            // DBに注文実績が一件も無い場合(エラー回避)
            if (_context.ChumonJisseki.Count() == 0)
            {
                // "選択された注文年月日-001"を渡す
                string newDay = inChumonDate.ToString("yyyyMMdd-");
                string newNum = "001";
                return newDay + newNum;
            }

            // 機能１：注文ID、選択注文日、実績内注文日、注文番号を設定する
            string queriedChumonId = await _context.ChumonJisseki.AsNoTracking()
                .OrderBy(x => x.ChumonId).Select(x => x.ChumonId).LastAsync();
            string selectChumonDate = inChumonDate.ToString("yyyyMMdd-");
            string queriedChumonDate = queriedChumonId.Substring(0, queriedChumonId.Length - 3);
            string chumonNumber = queriedChumonId.Substring(queriedChumonId.Length - 3);

            // 機能１－１：選択された注文日で注文されていない場合、注文番号を"001"に設定する
            // 機能１－２：選択された注文日で注文されている場合、注文実績内で最も最後の注文IDに1追加した文字列(3桁)に設定する
            chumonNumber = selectChumonDate == queriedChumonDate ? (uint.Parse(chumonNumber) + 1).ToString("D3") : "001";

            // "注文年月日-注文番号三桁"を渡す(その日の注文実績数が1000以上の場合nullを渡す)
            return uint.Parse(chumonNumber) >= 999 ? null : selectChumonDate + chumonNumber;
        }
    }
}
