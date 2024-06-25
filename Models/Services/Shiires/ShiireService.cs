using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Properties.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            // 処理１：仕入実績を検索する
            // 処理２：倉庫在庫を検索する
            // 戻り値：検索結果、又は作成した仕入実績、倉庫在庫を格納した更新用VIewModel

            // 処理１：仕入実績を検索する
            IList<ShiireJisseki> queryShiireJissekis = await Shiire.ShiireQuery(inChumonId);
            // 処理１－１：見つからなかった場合
            if (queryShiireJissekis.Count == 0)
            {
                // 処理１－１－１：対応する注文実績明細が見つかった場合
                if (await Shiire.ChumonJissekiMeisaiQuery(inChumonId) != null)
                {
                    // 処理１－１－１－１；新規仕入実績を作成する
                    queryShiireJissekis = await Shiire.ShiireCreate(inChumonId);
                }
                // 処理１－１－２：見つからなかった場合
                else
                {
                    // エラー「注文されていません」
                    throw new Exception("注文されていません");
                }
            }
            // 処理１－２：検索結果、又は作成後の仕入実績をソートする
            Shiire.ShiireJissekis = queryShiireJissekis.OrderBy(sj => sj.ShohinId).ToList();

            // 処理２：倉庫在庫を検索する
            IList<SokoZaiko> querySokoZaikos = await Shiire.ZaikoQuery(queryShiireJissekis.First().ShiireSakiId);
            // 処理２－１：見つからなかった場合
            if (querySokoZaikos.Count == 0)
            {
                // 処理２－１－１：新規倉庫在庫を作成する
                querySokoZaikos = await Shiire.ZaikoCreate(queryShiireJissekis.First().ShiireSakiId);
            }
            // 処理２－２：検索結果、又は作成後の倉庫在庫をソートする
            Shiire.SokoZaikos = querySokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 検索結果、又は作成した仕入実績、倉庫在庫を格納した更新用ViewModelを作成し渡す
            return new ShiireUpdateViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                IsNormal = false,
                Remark = string.Empty,
            };
        }

        // 仕入実績、倉庫在庫更新
        // inShiireUpdateViewModel
        public async Task<ShiireUpdateViewModel> ShiireCommit(ShiireUpdateViewModel inShiireUpdateViewModel)
        {
            // 処理１：仕入実績、倉庫在庫を微修正する
            // 処理２：仕入実績、倉庫在庫を更新する
            // 戻り値：更新後の仕入実績、倉庫在庫を格納した更新用ViewModel

            // 処理１：入力後の仕入実績、倉庫在庫を微修正する
            ShiireUpdateViewModel getShiireUpdateViewModel = inShiireUpdateViewModel;
            IList<ShiireMaster> queryShiireMasters = await _context.ShiireMaster
                .Where(shi => shi.ShiireSakiId == getShiireUpdateViewModel.SokoZaikos[0].ShiireSakiId)
                .OrderBy(shi => shi.ShohinId)
                .ToListAsync();
            bool isChange = false;
            for (int shiiresCounter = 0; shiiresCounter < getShiireUpdateViewModel.ShiireJissekis.Count; shiiresCounter++)
            {
                // マイナス入力対応
                if (getShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu < 0)
                {
                    getShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu = 0;
                }
                // 小数対策
                if (Regex.IsMatch(getShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu.ToString(), @"\.\d{3,}"))
                {
                    throw new Exception("小数点三桁以上");
                }

                // 処理１－１：入力前と入力後を比較して変化しているか判定する
                if (Shiire.ShiireJissekis[shiiresCounter].NonyuSu != getShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu)
                {
                    isChange = true;
                }
                // 処理１－２：対応する仕入マスタを設定する
                getShiireUpdateViewModel.SokoZaikos[shiiresCounter].ShiireMaster = queryShiireMasters[shiiresCounter];
            }

            // 処理２：納入数に応じて注文残、倉庫在庫数を変動する
            ShiireUpdateViewModel updateShiireViewModel = await Shiire.ChumonZanBalance(getShiireUpdateViewModel);
            // 処理２－１：仕入実績、倉庫在庫を更新する
            IList<ShiireJisseki> updateShiireJissekis = await Shiire.ShiireUpdate(updateShiireViewModel.ShiireJissekis);
            IList<SokoZaiko> updateSokoZaikos = await Shiire.ZaikoUpdate(updateShiireViewModel.SokoZaikos);

            // 処理２－２：DBを更新する
            await _context.SaveChangesAsync();

            // 処理２－３：仕入実績、倉庫在庫に対応する別テーブルをインクルードする
            IList<ShiireJisseki> includeShiireJissekis = await IncludeShiireJissekis(updateShiireJissekis);
            IList<SokoZaiko> includeSokoZaikos = await IncludeSokoZaikos(updateSokoZaikos);

            // 処理２－４：更新後の仕入実績、倉庫在庫を設定する
            Shiire.ShiireJissekis = includeShiireJissekis.OrderBy(sj => sj.ShohinId).ToList();
            Shiire.SokoZaikos = includeSokoZaikos.OrderBy(sok => sok.ShohinId).ToList();

            // 更新後の仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireUpdateViewModel()
            {
                ShiireJissekis = Shiire.ShiireJissekis,
                SokoZaikos = Shiire.SokoZaikos,
                // 更新したかを判定する
                IsNormal = isChange ? true : false,
                // 表示する文字列
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
                //// 処理１－１：対応する注文実績明細、注文実績、仕入先マスタ、仕入マスタ、商品マスタをインクルードする
                ChumonJissekiMeisai includeChumonJissekiMeisai = await _context.ChumonJissekiMeisai
                    .Where(mei => mei.ChumonId == shiire.ChumonId && mei.ShiireSakiId == shiire.ShiireSakiId &&
                    mei.ShiirePrdId == shiire.ShiirePrdId && mei.ShohinId == shiire.ShohinId)
                    .Include(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(saki => saki.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstAsync();
                ShiireJisseki queryShiireJisseki = shiire;
                queryShiireJisseki.ChumonJissekiMeisai = includeChumonJissekiMeisai;

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
                ShiireMaster includeShiireMaster = await _context.ShiireMaster
                    .Where(sm => sm.ShiireSakiId == zaiko.ShiireSakiId && sm.ShiirePrdId == zaiko.ShiirePrdId &&
                    sm.ShohinId == zaiko.ShohinId)
                    .Include(sm => sm.ShohinMaster)
                    .FirstAsync();
                SokoZaiko querySokoZaiko = zaiko;
                querySokoZaiko.ShiireMaster = includeShiireMaster;

                // 処理１－２：インクルード後倉庫在庫リストに追加する
                querySokoZaikos.Add(querySokoZaiko);
            }

            // インクルード後倉庫在庫リストを渡す
            return querySokoZaikos;
        }
    }
}
