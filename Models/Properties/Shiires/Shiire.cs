using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Shiires
{
    // 仕入
    public class Shiire : IShiire
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 仕入実績リスト
        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        // 倉庫在庫リスト
        public IList<SokoZaiko> SokoZaikos { get; set; }

        // コンストラクタ
        public Shiire(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
        }

        /*
         * 仕入実績問い合わせ
         * inChumonId：選択した注文コード
         */
        public IList<ShiireJisseki> ShiireToiawase(string inChumonId)
        {
            /*
             * 注文コードを基にDBに対象の仕入実績があるかを判定
             * ある場合は対象の仕入実績を戻り値に渡す
             * 無い場合はnullを渡す
             */

            // 仕入実績を問い合わせる
            ShiireJissekis = _context.ShiireJisseki
                .Where(sj => sj.ChumonId == inChumonId)
                .Include(sj => sj.ChumonJissekiMeisai)
                .ThenInclude(mei => mei.ChumonJisseki)
                .ThenInclude(chu => chu.ShiireSakiMaster)
                .ThenInclude(ss => ss.ShiireMasters)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToList();

            // 取得した仕入実績リストを渡す(無い場合はnullを渡す)
            return ShiireJissekis;
        }

        /*
         * 仕入実績作成
         * inChumonId：選択した注文コード
         */
        public IList<ShiireJisseki> ShiireSakusei(string inChumonId)
        {
            /*
             * 選択された注文コードを基に仕入実績を登録されている商品数分新規作成する
             * 実行時の日時を取得、実行時の日付を取得、仕入SEQを設定
             * 仕入先コードを取得、仕入商品コードリスト商品コードリストを取得
             * 納入数を0に設定
             * 空の仕入実績を複数新規作成する
             * 作成した仕入実績リストを戻り値に渡す
             */

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
                // 空の仕入実績追加
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
                // 対応する注文実績明細をインクルード
                ShiireJissekis[i].ChumonJissekiMeisai = _context.ChumonJissekiMeisai
                    .Where(mei => mei.ChumonId == inChumonId && mei.ShiireSakiId == shiireSakiId &&
                    mei.ShiirePrdId == shiirePrdIds[i] && mei.ShohinId == shohinIds[i])
                    .Include(mei => mei.ChumonJisseki)
                    .ThenInclude(chu => chu.ShiireSakiMaster)
                    .ThenInclude(sak => sak.ShiireMasters)
                    .ThenInclude(sm => sm.ShohinMaster)
                    .FirstOrDefault();
            }

            // 作成した仕入実績リストを渡す
            return ShiireJissekis;
        }

        /*
         * 仕入実績更新
         * inShiireJissekis：入力した納入数が格納された仕入実績リスト
         */
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis)
        {
            /*
             * 初期表示されていた内容と入力後の内容が変わっているかを判定
             * 変わっていない場合、初期表示されていた値を戻り値に渡す
             * 変わっている場合、今回の仕入実績が既存データの更新か新規作成かを判定
             * 更新の場合、既存データの納入数を変動させる
             * 新規作成の場合、複数の仕入実績をDBに追加する
             * 更新後又は追加後の仕入実績リストを戻り値に渡す
             */

            // 前回と比べて納入数が変化しているかを判定
            bool changeFlag = false;
            for (int i = 0; i < inShiireJissekis.Count; i++)
            {
                // 納入数が違う場合更新処理に移動
                if (ShiireJissekis[i].NonyuSu != inShiireJissekis[i].NonyuSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            // 更新処理
            if (changeFlag)
            {
                // DBに対象の仕入実績があるかを検索
                IList<ShiireJisseki> shiireJissekis = ShiireJissekis;

                shiireJissekis = _context.ShiireJisseki
                    .Where(sj => sj.ShiireSakiId == inShiireJissekis.First().ShiireSakiId &&
                    sj.ChumonId == inShiireJissekis.First().ChumonId)
                    .OrderBy(sj => sj.ShohinId)
                    .ToList();

                // DBにある場合
                if (shiireJissekis.Count != 0)
                {
                    // 納入数を入力後のデータと同期
                    for (int i = 0; i < shiireJissekis.Count; i++)
                    {
                        shiireJissekis[i].NonyuSu = inShiireJissekis[i].NonyuSu;
                    }

                    // 仕入実績リストを更新
                    ShiireJissekis = shiireJissekis;
                }
                else
                {
                    // 仕入実績リストを追加
                    for (int i = 0; i < inShiireJissekis.Count; i++)
                    {
                        inShiireJissekis[i].ShiireDateTime = DateTime.SpecifyKind(inShiireJissekis[i].ShiireDateTime, DateTimeKind.Utc);
                        _context.ShiireJisseki.Add(inShiireJissekis[i]);
                    }

                    // 仕入実績を更新
                    ShiireJissekis = inShiireJissekis;
                }
            }

            // 更新又は追加後の仕入実績を渡す
            return ShiireJissekis;
        }

        /*
         * 倉庫在庫問い合わせ
         * inShiireSakiId：指定した仕入実績の仕入先コード
         */
        public IList<SokoZaiko> ZaikoToiawase(string inShiireSakiId)
        {
            /*
             * 指定した仕入実績の仕入先コードの倉庫在庫がDBにあるかを判定
             * ある場合は対象の倉庫在庫を戻り値に渡す
             * 無い場合はnullを戻り値に渡す
             */

            // 倉庫在庫を検索
            SokoZaikos = _context.SokoZaiko
                .Where(soko => soko.ShiireSakiId == inShiireSakiId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToList();

            // 対象の倉庫在庫を渡す(無い場合はnullを渡す)
            return SokoZaikos;
        }

        /*
         * 倉庫在庫作成
         * inShiireSakiId：指定した仕入実績の仕入先コード
         */
        public IList<SokoZaiko> ZaikoSakusei(string inShiireSakiId)
        {
            /*
             * 選択した仕入先コードを基に対応する倉庫在庫を新規作成
             * 仕入先コードが同じ仕入マスタをDBから取得
             * 空の倉庫在庫を複数追加
             * 空の倉庫在庫リストを戻り値に渡す
             */

            // 仕入マスタを設定
            var shiireMastars = _context.ShiireMaster
                .Where(sm => sm.ShiireSakiId == inShiireSakiId)
                .ToList();

            // 空の倉庫在庫を複数追加
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

            // 空の倉庫在庫リストを渡す
            return SokoZaikos;
        }

        /*
         * 倉庫在庫更新
         * inSokoZaikos：変動した在庫数が格納された倉庫在庫リスト
         */
        public IList<SokoZaiko> ZaikoUpdate(IList<SokoZaiko> inSokoZaikos)
        {
            /*
             * 初期表示されていた内容と入力後の内容が変わっているかを判定
             * 変わっていない場合、初期表示されていた値を戻り値に渡す
             * 変わっている場合、今回の倉庫在庫が既存データの更新か新規作成かを判定
             * 更新の場合、既存データの倉庫在庫数を変動させる
             * 新規作成の場合、複数の倉庫在庫をDBに追加する
             * 更新後又は追加後の倉庫在庫リストを戻り値に渡す
             */

            // 前回と比べて在庫数が変化しているかを判定
            bool changeFlag = false;
            for (int i = 0; i < inSokoZaikos.Count; i++)
            {
                // 在庫数が違う場合更新処理に移動
                if (SokoZaikos[i].SokoZaikoSu != inSokoZaikos[i].SokoZaikoSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            // 更新処理
            if (changeFlag)
            {
                // DBに対象の倉庫在庫があるかを検索
                IList<SokoZaiko> sokoZaikos = SokoZaikos;

                sokoZaikos = _context.SokoZaiko
                    .Where(sz => sz.ShiireSakiId == inSokoZaikos.First().ShiireSakiId)
                    .OrderBy(sz => sz.ShohinId)
                    .ToList();

                // DBにある場合
                if (sokoZaikos.Count != 0)
                {
                    // 倉庫在庫数を入力後のデータと同期
                    for (int i = 0; i < sokoZaikos.Count; i++)
                    {
                        sokoZaikos[i].SokoZaikoSu = inSokoZaikos[i].SokoZaikoSu;
                    }

                    // 倉庫在庫を更新
                    SokoZaikos = sokoZaikos;
                }
                // 追加
                else
                {
                    // 複数の倉庫在庫を追加
                    for (int i = 0; i < inSokoZaikos.Count; i++)
                    {
                        _context.SokoZaiko.Add(inSokoZaikos[i]);
                    }

                    // 倉庫在庫を更新
                    SokoZaikos = inSokoZaikos;
                }
            }

            // 更新後又は追加後の倉庫在庫リストを渡す
            return SokoZaikos;
        }

        /*
         * 注文実績明細問い合わせ
         * inChumonId：選択された注文コード
         */
        public bool ChumonJissekiMeisaiToiawase(string inChumonId)
        {
            /*
             * 注文実績明細が無い場合false(エラー回避)
             * 選択された注文コードを持つ注文実績明細があるかを判定
             * 注文されている場合は仕入実績を新規作成する
             * 注文されていない場合はエラー表示する
             */

            // 注文実績明細が無い場合エラー
            if (_context.ChumonJissekiMeisai.Count() == 0) return false;

            // 対応する注文実績明細があるかを検索
            var check = _context.ChumonJissekiMeisai.Where(mei => mei.ChumonId == inChumonId).FirstOrDefault();

            // 注文されている場合仕入実績を新規作成
            if (check != null) return true;
            // 注文されていない場合エラー
            else return false;
        }

        /*
         * 注文残倉庫在庫数変動
         * inShiireViewModel：入力された内容が格納された仕入実績、倉庫在庫更新用ViewModel
         */
        public ShiireViewModel ChumonZanBalance(ShiireViewModel inShiireViewModel)
        {
            /*
             * 入力された納入数を基に注文残と倉庫在庫数を変動
             * 注文残を参照するために対応する注文実績明細を取得
             * 入力された納入数が注文残以上の場合、納入数を調整
             * 初期表示されていた納入数と比較して注文残、倉庫在庫数の変動量を設定
             * 注文残、倉庫在庫数を変動
             * 変動後の仕入実績、倉庫在庫を格納したViewModelを作成し戻り値に渡す
             */

            // 注文実績明細取得
            var meisai = _context.ChumonJissekiMeisai
                .Where(mei => mei.ShiireSakiId == inShiireViewModel.ShiireJissekis.First().ShiireSakiId &&
                mei.ChumonId == inShiireViewModel.ShiireJissekis.First().ChumonId)
                .OrderBy(mei => mei.ShohinId)
                .ToList();

            // 注文残、倉庫在庫数を変動
            for (int i = 0; i < inShiireViewModel.ShiireJissekis.Count; i++)
            {
                // 注文残以上の納入不可
                inShiireViewModel.ShiireJissekis[i].NonyuSu = inShiireViewModel.ShiireJissekis[i].NonyuSu - ShiireJissekis[i].NonyuSu <= meisai[i].ChumonZan ?
                    inShiireViewModel.ShiireJissekis[i].NonyuSu : ShiireJissekis[i].NonyuSu + meisai[i].ChumonZan;

                // 前回の納入数と比較して注文残、在庫数の変動量を設定
                decimal transNum = inShiireViewModel.ShiireJissekis[i].NonyuSu - ShiireJissekis[i].NonyuSu;

                // 注文残変動
                meisai[i].ChumonZan -= transNum;
                // 在庫数変動
                inShiireViewModel.SokoZaikos[i].SokoZaikoSu += transNum;
            }

            // 変動後の注文実績明細は後ほど行うDB更新で反映される
            // 変動後の仕入実績、倉庫在庫を格納したViewModelを作成し渡す
            return new ShiireViewModel()
            {
                ShiireJissekis = inShiireViewModel.ShiireJissekis,
                SokoZaikos = inShiireViewModel.SokoZaikos,
            };
        }
    }
}
