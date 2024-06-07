using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC_Context;

namespace ConvenienceMVC.Models.Properties.Zaikos
{
    public class Zaiko : IZaiko
    {
        private readonly ConvenienceMVCContext _context;

        public Zaiko(ConvenienceMVCContext context)
        {
            _context = context;
        }

        /*
         * 倉庫在庫絞り込み
         * inNarrowCodes：絞り込みキーリスト
         */
        public IList<SokoZaiko> NarrowSokoZaiko(IList<string>? inNarrowCodes)
        {
            /*
             * 絞り込みキーリストを基に対応する倉庫在庫を取得する
             * 取得したデータを重複無しに変換し、戻り値に渡す
             */

            // 複数ラムダ式設定
            IList<Func<SokoZaiko, bool>> orderByExpression = new List<Func<SokoZaiko, bool>>();
            // 対応倉庫在庫格納
            IList<IList<SokoZaiko>> sokoZaikosList = new List<IList<SokoZaiko>>();
            // 最終倉庫在庫
            IList<SokoZaiko> sokoZaikos = new List<SokoZaiko>();

            // 空のラムダ式追加
            for (int i = 0; i < inNarrowCodes.Count; i++)
            {
                orderByExpression.Add(default);
            }

            // 絞り込みデータ取得
            for (int i = 0; i < orderByExpression.Count; i++)
            {
                orderByExpression[i] = SetNarrowOrder(orderByExpression[i], inNarrowCodes[i]);
                sokoZaikosList.Add(_context.SokoZaiko.Where(orderByExpression[i]).ToList());
            }

            // 重複無しに変換
            foreach (var zaikos in sokoZaikosList)
            {
                for (int i = 0; i < zaikos.Count; i++)
                {
                    // 重複確認
                    if (sokoZaikos.Any(x => x.ShiireSakiId == zaikos[i].ShiireSakiId && x.ShohinId == zaikos[i].ShohinId)) continue;
                    sokoZaikos.Add(zaikos[i]);
                }
            }

            // 最終倉庫在庫を渡す
            return sokoZaikos;
        }

        /*
         * 倉庫在庫並び替え
         * inSokoZaikos：絞り込み済みの倉庫在庫
         * inSortCodes：ソートキーリスト
         * inDescendingFlags：降順フラグリスト
         */
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, IList<string?> inSortCodes, IList<bool> inDescendingFlags)
        {
            /*
             * ソートキーリストを基に倉庫在庫を並び替える
             * 並び替えた倉庫在庫を戻り値に渡す
             */

            // 複数ラムダ式設定
            IList<Func<SokoZaiko, object>> orderByExpression = new List<Func<SokoZaiko, object>>();

            // 空のラムダ式追加
            for (int i = 0; i < inDescendingFlags.Count; i++)
            {
                orderByExpression.Add(default);
            }

            // ソート設定
            for (int i = 0; i < orderByExpression.Count; i++)
            {
                orderByExpression[i] = SetSortOrder(orderByExpression[i], inSortCodes[i]);
            }

            // 在庫ソート
            inSokoZaikos = inSokoZaikos
                .OrderBy(orderByExpression[0], !inDescendingFlags[0] ? Comparer<object>.Default :
                Comparer<object>.Create((x, y) => -Comparer<object>.Default.Compare(x, y)))
                .ThenBy(orderByExpression[1], !inDescendingFlags[1] ? Comparer<object>.Default :
                Comparer<object>.Create((x, y) => -Comparer<object>.Default.Compare(x, y)))
                .ThenBy(orderByExpression[2], !inDescendingFlags[2] ? Comparer<object>.Default :
                Comparer<object>.Create((x, y) => -Comparer<object>.Default.Compare(x, y)))
                .ToList();

            // ソート後の倉庫在庫を戻り値に渡す
            return inSokoZaikos;
        }

        /*
         * 絞り込みラムダ式設定
         * inOrder：空のラムダ式
         * inNarrowCode：絞り込みキー
         */
        private Func<SokoZaiko, bool> SetNarrowOrder(Func<SokoZaiko, bool> inOrder, string? inNarrowCode)
        {
            /*
             * 絞り込みキーを基にラムダ式を変更
             * 変更したラムダ式を戻り値に渡す
             */

            switch (inNarrowCode)
            {
                case "SS001":
                    inOrder = soko => soko.ShiireSakiId == inNarrowCode;
                    break;
                case "SS002":
                    inOrder = soko => soko.ShiireSakiId == inNarrowCode;
                    break;
                case "SS003":
                    inOrder = soko => soko.ShiireSakiId == inNarrowCode;
                    break;
                case "SS004":
                    inOrder = soko => soko.ShiireSakiId == inNarrowCode;
                    break;
                case "SS005":
                    inOrder = soko => soko.ShiireSakiId == inNarrowCode;
                    break;

                case "パプリカカレー":
                    inOrder = soko => soko.ShiireMaster.ShohinMaster.ShohinName == inNarrowCode;
                    break;
                case "DXファイアフライザー":
                    inOrder = soko => soko.ShiireMaster.ShohinMaster.ShohinName == inNarrowCode;
                    break;
                case "Hey！落ち着け！":
                    inOrder = soko => soko.ShiireMaster.ShohinMaster.ShohinName == inNarrowCode;
                    break;
                case "分割されたハッピー":
                    inOrder = soko => soko.ShiireMaster.ShohinMaster.ShohinName == inNarrowCode;
                    break;

                default:
                    break;
            }

            return inOrder;
        }

        /*
         * 並び替えラムダ式設定
         * inOrder：空のラムダ式
         * inSortCode：ソートキー
         */
        private Func<SokoZaiko, object> SetSortOrder(Func<SokoZaiko, object> inOrder, string? inSortCode)
        {
            /*
             * ソートキーを基にラムダ式を変更
             * 変更したラムダ式を戻り値に渡す
             */

            switch (inSortCode)
            {
                case "ShiireSakiId":
                    inOrder = soko => soko.ShiireSakiId;
                    break;

                case "ShiirePrdId":
                    inOrder = soko => soko.ShiirePrdId;
                    break;

                case "ShohinId":
                    inOrder = soko => soko.ShohinId;
                    break;

                case "ShohinName":
                    inOrder = soko => soko.ShiireMaster.ShohinMaster.ShohinName;
                    break;

                case "SokoZaikoCaseSu":
                    inOrder = soko => soko.SokoZaikoCaseSu;
                    break;

                case "SokoZaikoSu":
                    inOrder = soko => soko.SokoZaikoSu;
                    break;

                case "LastShiireDate":
                    inOrder = soko => soko.LastShiireDate;
                    break;

                case "LastDeliveryDate":
                    inOrder = soko => soko.LastDeliveryDate;
                    break;

                default:
                    inOrder = soko => soko.ShiireSakiId;
                    break;
            }

            return inOrder;
        }
    }
}
