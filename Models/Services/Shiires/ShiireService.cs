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

        /*
         * 仕入実績、倉庫在庫設定
         * inChumonId：選択された注文コード
         */
        public ShiireViewModel ShiireSetting(string inChumonId)
        {
            /*
             * 選択された注文コードを基に対応する仕入実績がDBにあるかを検索
             * ある場合、見つかった仕入実績を使用
             * 無い場合、選択された注文コードを基に対応する注文実績明細がDBにあるかを検索
             * ある場合、仕入実績を新規作成
             * 無い場合、エラー表示
             * 
             * 見つかった仕入実績又は新規作成した仕入実績の仕入先コードを基に倉庫在庫がDBにあるかを検索
             * ある場合、見つかった倉庫在庫を使用
             * 無い場合、倉庫在庫を新規作成
             * 
             * 作成した又は見つけた仕入実績、倉庫在庫を格納したViewModelを作成し戻り値に渡す
             */

            // 仕入実績を検索
            IList<ShiireJisseki> queryShiireJissekis = Shiire.ShiireQuery(inChumonId);
            // 仕入実績を検索して見つからなかった場合
            if (queryShiireJissekis.Count == 0)
            {
                // 注文実績明細があるかどうかを調べる
                // 注文はされているけどまだ仕入れていない可能性もある為
                // 注文はされている場合
                if (Shiire.IsChumonJissekiMeisai(inChumonId))
                {
                    // 仕入実績新規作成
                    queryShiireJissekis = Shiire.ShiireCreate(inChumonId);
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
            IList<SokoZaiko> querySokoZaikos = Shiire.ZaikoQuery(queryShiireJissekis.First().ShiireSakiId);
            // 倉庫在庫を検索して見つからなかった場合
            if (querySokoZaikos.Count == 0)
            {
                // 倉庫在庫新規作成
                querySokoZaikos = Shiire.ZaikoCreate(queryShiireJissekis.First().ShiireSakiId);
            }
            Shiire.SokoZaikos = querySokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 作成した又は見つけた仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireViewModel()
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
        public async Task<ShiireViewModel> ShiireCommit(ShiireViewModel inShiireViewModel)
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
            ShiireViewModel updateShiireViewModel = Shiire.ChumonZanBalance(inShiireViewModel);
            // 仕入実績更新
            IList<ShiireJisseki>? updateShiireJissekis = Shiire.ShiireUpdate(updateShiireViewModel.ShiireJissekis);
            // 在庫倉庫更新
            IList<SokoZaiko>? updateSokoZaikos = Shiire.ZaikoUpdate(updateShiireViewModel.SokoZaikos);

            // DB更新
            await _context.SaveChangesAsync();

            // 仕入実績対応要素インクルード
            updateShiireJissekis = IncludeShiireJissekis(updateShiireJissekis);
            // 倉庫在庫対応要素インクルード
            updateSokoZaikos = IncludeSokoZaikos(updateSokoZaikos);

            // 仕入実績設定
            Shiire.ShiireJissekis = updateShiireJissekis.OrderBy(sj => sj.ShohinId).ToList();
            // 倉庫在庫設定
            Shiire.SokoZaikos = updateSokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 入力前後で納入数が変動していた場合、更新完了を表示するようにする
            // 更新後の仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                IsNormal = isChange ? true : false,
                Remark = isChange ? "更新完了" : string.Empty,
            };
        }

        /*
         * 仕入実績対応要素インクルード
         * inShiireJissekis：更新後の仕入実績
         */
        private IList<ShiireJisseki> IncludeShiireJissekis(IList<ShiireJisseki>? inShiireJissekis)
        {
            /*
             * 更新後の仕入実績に対応する要素をインクルード
             * インクルード後の仕入実績を戻り値に渡す
             */

            // インクルード結果を格納するリストを作成
            IList<ShiireJisseki> queryShiireJissekis = new List<ShiireJisseki>();
            // 対応する要素をインクルードし、リストに追加
            foreach (ShiireJisseki shiire in inShiireJissekis)
            {
                // 対応する要素をインクルード
                ShiireJisseki queryShiireJisseki = _context.ShiireJisseki
                    .Where(sj => sj.ChumonId == shiire.ChumonId && sj.ShiireDate == shiire.ShiireDate &&
                    sj.SeqByShiireDate == shiire.SeqByShiireDate && sj.ShiireSakiId == shiire.ShiireSakiId &&
                    sj.ShiirePrdId == shiire.ShiirePrdId)
                    .Include(sj => sj.ChumonJissekiMeisai)
                    .ThenInclude(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(saki => saki.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();

                // リストに追加
                queryShiireJissekis.Add(queryShiireJisseki);
            }

            // インクルード後の仕入実績を渡す
            return queryShiireJissekis;
        }
        /*
         * 倉庫在庫対応要素インクルード
         * inSokoZaikos：更新後の倉庫在庫
         */
        private IList<SokoZaiko> IncludeSokoZaikos(IList<SokoZaiko>? inSokoZaikos)
        {
            /*
             * 更新後の倉庫在庫に対応する要素をインクルード
             * インクルード後の倉庫在庫を戻り値に渡す
             */

            // インクルード結果を格納するリストを作成
            IList<SokoZaiko> querySokoZaikos = new List<SokoZaiko>();
            // 対応する要素をインクルードし、リストに追加
            foreach (SokoZaiko zaiko in inSokoZaikos)
            {
                // 対応する要素をインクルード
                SokoZaiko querySokoZaiko = _context.SokoZaiko
                    .Where(sk => sk.ShiireSakiId == zaiko.ShiireSakiId && sk.ShiirePrdId == zaiko.ShiirePrdId &&
                    sk.ShohinId == zaiko.ShohinId)
                    .Include(sk => sk.ShiireMaster)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();

                // リストに追加
                querySokoZaikos.Add(querySokoZaiko);
            }

            // インクルード後の倉庫在庫を渡す
            return querySokoZaikos;
        }
    }
}
