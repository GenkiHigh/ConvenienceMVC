using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Zaikos;

namespace ConvenienceMVC.Models.Properties.Zaikos
{
    public class Zaiko : IZaiko
    {
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, IList<string?> inSortCodes, IList<bool> inDescendingFlags)
        {
            // 複数ラムダ式設定
            IList<Func<SokoZaiko, object>> orderByExpression = new List<Func<SokoZaiko, object>>();
            for (int i = 0; i < inDescendingFlags.Count; i++)
            {
                orderByExpression.Add(default);
            }

            // ソート設定
            for (int i = 0; i < orderByExpression.Count; i++)
            {
                orderByExpression[i] = SetOrder(orderByExpression[i], inSortCodes[i]);
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

            return inSokoZaikos;
        }

        // ソート設定
        private Func<SokoZaiko, object> SetOrder(Func<SokoZaiko, object> inOrder, string? inSortCode)
        {
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
