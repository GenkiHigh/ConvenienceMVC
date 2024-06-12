using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC.Models.Properties.Chumons;
using ConvenienceMVC.Models.Views.Chumons;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Chumons
{
    // 注文サービス
    public class ChumonService : IChumonService
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 注文インターフェース
        public IChumon Chumon { get; set; }

        // コンストラクタ
        public ChumonService(ConvenienceMVCContext context)
        {
            // DBコンテキスト設定
            _context = context;
            // 注文インスタンス生成
            Chumon = new Chumon(_context);
        }

        /*
         * 注文実績設定
         * inShiireSakiId：選択された仕入先コード
         * inChumonDate：選択された注文日
         */
        public async Task<ChumonViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate)
        {
            /*
             * 選択された仕入先コード、注文日を元に注文実績を問い合わせる
             * 問い合わせた結果選択した要素を持つ注文実績がある場合、仕入先マスタをインクルードする
             * 選択した要素を持つ注文実績が無い場合、注文実績を新規作成する
             * 注文実績を検索、作成後のデータに更新する
             * 更新後の注文実績を持つ注文実績更新用ViewModelを戻り値に渡す
             */

            // 注文実績を問い合わせる
            ChumonJisseki queriedChumonJisseki = Chumon.ChumonQuery(inShiireSakiId, inChumonDate);

            // 指定した注文実績が存在している場合
            if (queriedChumonJisseki != default)
            {
                // 注文実績に必要な要素をインクルード
                queriedChumonJisseki = ChumonJissekiInclude(queriedChumonJisseki);

                IList<ChumonJissekiMeisai> meisais = await _context.ChumonJissekiMeisai
                    .Where(mei => mei.ChumonId == queriedChumonJisseki.ChumonId)
                    .OrderBy(mei => mei.ShohinId).ToListAsync();

                queriedChumonJisseki.ChumonJissekiMeisais = meisais;
            }
            // 指定した注文実績が存在していない場合
            else
            {
                // 注文実績を新規作成
                queriedChumonJisseki = Chumon.ChumonCreate(inShiireSakiId, inChumonDate);
            }

            // 注文実績更新
            Chumon.ChumonJisseki = queriedChumonJisseki;

            // 注文実績更新用ViewModelを作成し渡す
            return new ChumonViewModel()
            {
                ChumonJisseki = Chumon.ChumonJisseki,
                IsNormal = false,
                Remark = string.Empty
            };
        }

        /*
         * 注文実績更新
         * inChumonViewModel：入力された注文数を格納した注文実績更新用ViewModel
         */
        public async Task<ChumonViewModel> ChumonCommit(ChumonViewModel inChumonViewModel)
        {
            /*
             * 初期表示されていた注文数と入力後の注文数を比較して注文数が変更されたかを判定
             * 入力後の注文残を入力後の注文数と同期
             * 入力後の注文実績を元にDBに追加、又はDBを更新
             * DBを更新
             * 注文実績に必要な要素をインクルード
             * 注文実績を更新
             * 注文実績更新用ViewModelを作成し渡す
             */

            // 注文数が変更されたかを判定
            bool isChange = false;
            for (int meisaisCounter = 0; meisaisCounter < inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais.Count; meisaisCounter++)
            {
                // 変更前の注文数と変更後の注文数を比較して変化しているかを判定
                if (Chumon.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu !=
                    inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu)
                {
                    isChange = true;
                }
                // 変更後の注文数残を同期
                inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan =
                    inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu;
            }
            // 注文更新
            // 前回か最新の結果が返ってくる
            ChumonJisseki updateChumonJisseki = Chumon.ChumonUpdate(inChumonViewModel.ChumonJisseki);

            // DB更新
            await _context.SaveChangesAsync();

            // 注文実績必要様をインクルード
            updateChumonJisseki = ChumonJissekiInclude(updateChumonJisseki);

            // 注文実績更新
            Chumon.ChumonJisseki = updateChumonJisseki;

            return new ChumonViewModel()
            {
                ChumonJisseki = updateChumonJisseki,
                IsNormal = isChange ? true : false,
                Remark = isChange ? "更新完了" : string.Empty,
            };
        }

        // 注文実績必要要素インクルード
        private ChumonJisseki ChumonJissekiInclude(ChumonJisseki inChumonJisseki)
        {
            ChumonJisseki queriedChumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ChumonId == inChumonJisseki.ChumonId && ch.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .Include(shs => shs.ShiireSakiMaster)
                .FirstOrDefault();

            queriedChumonJisseki.ChumonJissekiMeisais = _context.ChumonJissekiMeisai
                .Where(mei => mei.ChumonId == queriedChumonJisseki.ChumonId)
                .OrderBy(mei => mei.ShohinId).ToList();

            return queriedChumonJisseki;
        }

        //private (bool, eError ChumonIdError) ChumonJissekiIsValid(ChumonJisseki inChumonJisseki)
        //{
        //    var chumonId = inChumonJisseki.ChumonId;
        //    var chumonDate = inChumonJisseki.ChumonDate;

        //    if (!Regex.IsMatch(chumonId, "^[0-9]{8}-[0-9]{3}$"))
        //    {
        //        return (false, eError.ChumonIdError);
        //    }
        //    else if (chumonDate == null || chumonDate <= (new DateOnly(1, 1, 1)))
        //    {
        //        return (false, eError.ChumonDateError);
        //    }

        //    foreach (var i in inChumonJisseki.ChumonJissekiMeisais)
        //    {
        //        if (i.ChumonId != chumonId)
        //        {
        //            return (false, eError.ChumonIdRelationError);
        //        }
        //        else if (i.ChumonSu == null)
        //        {
        //            return (false, eError.ChumonSuIsNull);
        //        }
        //        else if (i.ChumonSu < 0)
        //        {
        //            return (false, eError.ChumonSuBadRange);
        //        }
        //        else if (i.ChumonZan == null)
        //        {
        //            return (false, eError.ChumonZanIsNull);
        //        }
        //        else if (i.ChumonSu < i.ChumonZan)
        //        {
        //            return (false, eError.SuErrorBetChumonSuAndZan);
        //        }
        //    }

        //    return (true, eError.NormalUpdate);
        //}
    }
}
