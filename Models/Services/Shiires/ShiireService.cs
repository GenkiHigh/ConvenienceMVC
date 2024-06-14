using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Properties.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Shiires
{
    // 仕入サービス
    public class ShiireService : IShiireService
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 仕入インターフェース
        public IShiire Shiire { get; set; }

        // コンストラクタ
        public ShiireService(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
            // 仕入のインスタンス生成
            Shiire = new Shiire(_context);
        }

        // 仕入実績、倉庫在庫設定
        // inChumonId：検索画面で選択された注文コード
        public async Task<ShiireUpdateViewModel> ShiireSetting(string inChumonId)
        {
            // 仕入実績を検索
            IList<ShiireJisseki> queryShiireJissekis = await Shiire.ShiireQuery(inChumonId);
            // 仕入実績を検索して見つからなかった場合
            if (queryShiireJissekis.Count == 0)
            {
                // 注文実績明細があるかどうかを調べる
                // 注文はされているけどまだ仕入れていない可能性もある為
                // 注文はされている場合
                if (await Shiire.ChumonJissekiMeisaiQuery(inChumonId) != null)
                {
                    // 仕入実績新規作成
                    queryShiireJissekis = await Shiire.ShiireCreate(inChumonId);
                }
                // 注文すらされていない場合
                else
                {
                    // エラー「注文されていません」
                    throw new Exception("注文されていません");
                }
            }
            Shiire.ShiireJissekis = queryShiireJissekis.OrderBy(sj => sj.ShohinId).ToList();

            // 倉庫在庫を検索
            IList<SokoZaiko> querySokoZaikos = await Shiire.ZaikoQuery(queryShiireJissekis.First().ShiireSakiId);
            // 倉庫在庫を検索して見つからなかった場合
            if (querySokoZaikos.Count == 0)
            {
                // 倉庫在庫新規作成
                querySokoZaikos = await Shiire.ZaikoCreate(queryShiireJissekis.First().ShiireSakiId);
            }
            Shiire.SokoZaikos = querySokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 作成した又は見つけた仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireUpdateViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                IsNormal = false,
                Remark = string.Empty,
            };
        }

        /*
         * 仕入実績、倉庫在庫更新
         * inShiireViewModel：入力したデータを格納したViewModel
         */
        public async Task<ShiireUpdateViewModel> ShiireCommit(ShiireUpdateViewModel inShiireViewModel)
        {
            /*
             * 初期表示されていた内容と入力後の内容を比較して変更されたかを判定　@①
             * 入力された納入数に応じて注文残、倉庫在庫数を変動
             * 仕入実績を更新
             * 倉庫在庫を更新
             * DBを更新
             * 仕入実績に対応する要素をインクルード
             * 倉庫在庫に対応する要素をインクルード
             * @①で変更されていた場合、更新完了を表示するようにする
             * 更新した仕入実績、倉庫在庫を格納したViewModelを作成し戻り値に渡す
             */

            IList<ShiireMaster> queryShiireMasters = await _context.ShiireMaster
                .Where(shi => shi.ShiireSakiId == inShiireViewModel.SokoZaikos.First().ShiireSakiId)
                .OrderBy(shi => shi.ShohinId)
                .ToListAsync();

            // 内容が更新されたか判断
            bool isChange = false;
            for (int shiiresCounter = 0; shiiresCounter < inShiireViewModel.ShiireJissekis.Count; shiiresCounter++)
            {
                // 納入数が変更されている場合、更新完了を画面で表示するようにする(下記参照)
                if (Shiire.ShiireJissekis[shiiresCounter].NonyuSu != inShiireViewModel.ShiireJissekis[shiiresCounter].NonyuSu)
                {
                    isChange = true;
                }
                inShiireViewModel.SokoZaikos[shiiresCounter].ShiireMaster = queryShiireMasters[shiiresCounter];
            }

            // 納入数に応じて注文残、倉庫在庫数変動
            ShiireUpdateViewModel updateShiireViewModel = await Shiire.ChumonZanBalance(inShiireViewModel);
            // 仕入実績更新
            IList<ShiireJisseki> updateShiireJissekis = await Shiire.ShiireUpdate(updateShiireViewModel.ShiireJissekis);
            // 在庫倉庫更新
            IList<SokoZaiko> updateSokoZaikos = await Shiire.ZaikoUpdate(updateShiireViewModel.SokoZaikos);

            // DB更新
            await _context.SaveChangesAsync();

            // 仕入実績対応要素インクルード
            updateShiireJissekis = await IncludeShiireJissekis(updateShiireJissekis);
            // 倉庫在庫対応要素インクルード
            updateSokoZaikos = await IncludeSokoZaikos(updateSokoZaikos);

            // 仕入実績設定
            Shiire.ShiireJissekis = updateShiireJissekis.OrderBy(sj => sj.ShohinId).ToList();
            // 倉庫在庫設定
            Shiire.SokoZaikos = updateSokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 入力前後で納入数が変動していた場合、更新完了を表示するようにする
            // 更新後の仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireUpdateViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                IsNormal = isChange ? true : false,
                Remark = isChange ? "更新完了" : string.Empty,
            };
        }

        // 仕入実績対応別テーブルインクルード
        // inShiireJissekis：インクルード元の仕入実績リスト
        private async Task<IList<ShiireJisseki>> IncludeShiireJissekis(IList<ShiireJisseki> inShiireJissekis)
        {
            // 処理１：対応する別テーブルをインクルードする
            // 戻り値：インクルード後仕入実績リスト

            // 処理１：対応する別テーブルをインクルードする
            IList<ShiireJisseki> queryShiireJissekis = new List<ShiireJisseki>();
            foreach (ShiireJisseki shiire in inShiireJissekis)
            {
                // 処理１－１：対応する注文実績明細、注文実績、仕入先マスタ、仕入マスタ、商品マスタをインクルードする
                ShiireJisseki queryShiireJisseki = await _context.ShiireJisseki
                    .Where(sj => sj.ChumonId == shiire.ChumonId && sj.ShiireDate == shiire.ShiireDate &&
                    sj.SeqByShiireDate == shiire.SeqByShiireDate && sj.ShiireSakiId == shiire.ShiireSakiId &&
                    sj.ShiirePrdId == shiire.ShiirePrdId)
                    .Include(sj => sj.ChumonJissekiMeisai)
                    .ThenInclude(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(saki => saki.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstAsync();
                // 処理１－２：インクルード後仕入実績リストに追加する
                queryShiireJissekis.Add(queryShiireJisseki);
            }

            // インクルード後仕入実績リストを渡す
            return queryShiireJissekis;
        }
        // 倉庫在庫対応別テーブルインクルード
        // inSokoZaiko：インクルード元の倉庫在庫リスト
        private async Task<IList<SokoZaiko>> IncludeSokoZaikos(IList<SokoZaiko> inSokoZaikos)
        {
            // 処理１：対応する別テーブルをインクルードする
            // 戻り値：インクルード後倉庫在庫リスト

            // 処理１：対応する別テーブルをインクルードする
            IList<SokoZaiko> querySokoZaikos = new List<SokoZaiko>();
            foreach (SokoZaiko zaiko in inSokoZaikos)
            {
                // 処理１－１：対応する仕入マスタ、商品マスタをインクルードする
                SokoZaiko querySokoZaiko = await _context.SokoZaiko
                    .Where(sk => sk.ShiireSakiId == zaiko.ShiireSakiId && sk.ShiirePrdId == zaiko.ShiirePrdId &&
                    sk.ShohinId == zaiko.ShohinId)
                    .Include(sk => sk.ShiireMaster)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstAsync();
                // 処理１－２：インクルード後倉庫在庫リストに追加する
                querySokoZaikos.Add(querySokoZaiko);
            }

            // インクルード後倉庫在庫リストを渡す
            return querySokoZaikos;
        }
    }
}
