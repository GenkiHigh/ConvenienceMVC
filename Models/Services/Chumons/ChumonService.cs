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
            // 注文インターフェースインスタンス生成
            Chumon = new Chumon(_context);
        }

        // 注文実績設定
        // inShiireSakiId：検索画面で選択された仕入先コード
        // inChumonDate：検索画面で選択された注文日
        public async Task<ChumonUpdateViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate)
        {
            // 処理１：注文実績を検索する
            // 処理２：注文実績を設定する
            // 戻り値：検索結果、又は新規作成した注文実績を格納した注文実績更新用ViewModel

            // 処理１：注文実績を検索する
            ChumonJisseki queriedChumonJisseki = await Chumon.ChumonQuery(inShiireSakiId, inChumonDate);
            // 処理１－１：注文実績がある場合
            if (queriedChumonJisseki != default)
            {
                // 処理１－１－１：注文実績に対応する別テーブルをインクルードする
                queriedChumonJisseki = await ChumonJissekiInclude(queriedChumonJisseki);
            }
            // 処理１－２：注文実績が無い場合
            else
            {
                // 処理１－２－１：注文実績を新規作成する
                queriedChumonJisseki = await Chumon.ChumonCreate(inShiireSakiId, inChumonDate);
            }

            // 処理２：検索した注文実績、又は新規作成した注文実績を渡す
            Chumon.ChumonJisseki = queriedChumonJisseki;

            // 注文実績更新用ViewModelを作成し渡す
            return new ChumonUpdateViewModel()
            {
                ChumonJisseki = Chumon.ChumonJisseki,
                IsNormal = false,
                Remark = string.Empty
            };
        }

        // 注文実績更新
        // inChumonUpdateViewModel：更新画面で入力された注文数を格納した注文実績更新用ViewModel
        public async Task<ChumonUpdateViewModel> ChumonCommit(ChumonUpdateViewModel inChumonUpdateViewModel)
        {
            // 処理１：取得した注文実績明細のデータを変動する
            // 処理２：注文実績を更新する
            // 処理３：DBを更新する
            // 処理４：注文実績に対応する別テーブルをインクルードする
            // 処理５：注文実績を設定する
            // 戻り値：更新後の注文実績を格納した注文実績更新用ViewModel

            // 処理１：取得した注文実績明細のデータを変動する
            bool isChange = false;
            ChumonUpdateViewModel getChumonUpdateViewModel = inChumonUpdateViewModel;
            for (int meisaisCounter = 0; meisaisCounter < getChumonUpdateViewModel.ChumonJisseki.ChumonJissekiMeisais.Count; meisaisCounter++)
            {
                // 処理１－１：入力前の注文数と入力後の注文数を比較する
                if (Chumon.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu !=
                    getChumonUpdateViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu)
                {
                    // 変化したことを記憶する
                    isChange = true;
                }
                // 処理１－２：入力後の注文数残を同期する
                getChumonUpdateViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonZan =
                    getChumonUpdateViewModel.ChumonJisseki.ChumonJissekiMeisais[meisaisCounter].ChumonSu;
            }

            // 処理２：注文実績を更新する
            ChumonJisseki updateChumonJisseki = await Chumon.ChumonUpdate(getChumonUpdateViewModel.ChumonJisseki);

            // 処理３：DBを更新する
            await _context.SaveChangesAsync();

            // 処理４：注文実績に対応する別テーブルをインクルードする
            ChumonJisseki includeChumonJisseki = await ChumonJissekiInclude(updateChumonJisseki);

            // 処理５：注文実績を設定する
            Chumon.ChumonJisseki = includeChumonJisseki;

            // 注文実績更新用ViewModelを作成し渡す
            return new ChumonUpdateViewModel()
            {
                ChumonJisseki = includeChumonJisseki,
                // 更新したかを判定する
                IsNormal = isChange ? true : false,
                // 表示する文字列
                Remark = isChange ? "更新完了" : string.Empty,
            };
        }

        // 注文実績対応別テーブルインクルード
        // inChumonJisseki：インクルード元の注文実績
        private async Task<ChumonJisseki> ChumonJissekiInclude(ChumonJisseki inChumonJisseki)
        {
            // 処理１：注文実績に対応する別テーブルをインクルードする
            // 処理２：注文実績明細をソートする
            // 戻り値：インクルード後の注文実績明細

            // 処理１：注文実績明細、仕入先マスタ、仕入マスタ、商品マスタをインクルードする
            ChumonJisseki includeChumonJisseki = await _context.ChumonJisseki
                .Where(ch => ch.ChumonId == inChumonJisseki.ChumonId && ch.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .Include(shs => shs.ShiireSakiMaster)
                .FirstAsync();

            // 処理２：注文実績明細を商品コードでソートする
            includeChumonJisseki.ChumonJissekiMeisais = await _context.ChumonJissekiMeisai
                .Where(mei => mei.ChumonId == includeChumonJisseki.ChumonId)
                .OrderBy(mei => mei.ShohinId).ToListAsync();

            // インクルード後の注文実績を渡す
            return includeChumonJisseki;
        }
    }
}
