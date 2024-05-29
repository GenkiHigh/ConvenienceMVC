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
        public ChumonViewModel ChumonSetting(string inShiireSakiId, DateOnly inChumonDate)
        {
            /*
             * 選択された仕入先コード、注文日を元に注文実績を問い合わせる
             * 問い合わせた結果選択した要素を持つ注文実績がある場合、仕入先マスタをインクルードする
             * 選択した要素を持つ注文実績が無い場合、注文実績を新規作成する
             * 注文実績を検索、作成後のデータに更新する
             * 更新後の注文実績を持つ注文実績更新用ViewModelを戻り値に渡す
             */

            // 空の注文実績を設定
            ChumonJisseki chumonJisseki;
            // 注文実績を問い合わせる
            chumonJisseki = Chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

            // 指定した注文実績が存在している場合
            if (chumonJisseki != default)
            {
                // 注文実績に必要な要素をインクルード
                chumonJisseki = IncludeChumonJisseki(chumonJisseki);

                var meisais = _context.ChumonJissekiMeisai.Where(mei => mei.ChumonId == chumonJisseki.ChumonId)
                    .OrderBy(mei => mei.ShohinId).ToList();

                chumonJisseki.ChumonJissekiMeisais = meisais;
            }
            // 指定した注文実績が存在していない場合
            else
            {
                // 注文実績を新規作成
                chumonJisseki = Chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
            }

            // 注文実績更新
            Chumon.ChumonJisseki = chumonJisseki;

            // 注文実績更新用ViewModelを作成し渡す
            return new ChumonViewModel()
            {
                ChumonJisseki = chumonJisseki,
                IsNormal = false,
                Remark = string.Empty
            };
        }

        /*
         * 注文実績更新
         * inChumonViewModel：入力された注文数を格納した注文実績更新用ViewModel
         */
        public ChumonViewModel ChumonCommit(ChumonViewModel inChumonViewModel)
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
            // 注文実績必要様をインクルード
            inChumonViewModel.ChumonJisseki = IncludeChumonJisseki(inChumonViewModel.ChumonJisseki);

            // 注文実績更新
            Chumon.ChumonJisseki = inChumonViewModel.ChumonJisseki;

            return new ChumonViewModel()
            {
                ChumonJisseki = inChumonViewModel.ChumonJisseki,
                IsNormal = changeFlag ? true : false,
                Remark = changeFlag ? "更新完了" : string.Empty,
            };
        }

        // 注文実績必要要素インクルード
        public ChumonJisseki IncludeChumonJisseki(ChumonJisseki inChumonJisseki)
        {
            ChumonJisseki chumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ChumonId == inChumonJisseki.ChumonId && ch.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .Include(shs => shs.ShiireSakiMaster)
                .FirstOrDefault();

            chumonJisseki.ChumonJissekiMeisais = _context.ChumonJissekiMeisai
                .Where(mei => mei.ChumonId == chumonJisseki.ChumonId)
                .OrderBy(mei => mei.ShohinId).ToList();

            return chumonJisseki;
        }
    }
}
