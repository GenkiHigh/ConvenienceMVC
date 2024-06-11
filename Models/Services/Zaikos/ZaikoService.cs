using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC.Models.Properties.Zaikos;
using ConvenienceMVC.Models.Views.Zaikos;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Zaikos
{
    public class ZaikoService : IZaikoService
    {
        private readonly ConvenienceMVCContext _context;
        private IZaiko zaiko;

        public ZaikoService(ConvenienceMVCContext context)
        {
            _context = context;
            zaiko = new Zaiko(_context);
        }

        /*
         * 表示倉庫在庫設定
         * inZaikoViewModel：ソートキー、絞り込みキーを格納したViewModel
         */
        public ZaikoViewModel SetDisplaySokoZaiko(ZaikoViewModel inZaikoViewModel)
        {
            /*
             * 全倉庫在庫を取得
             * 絞り込みキーが1つ以上ある場合、倉庫在庫絞り込み
             * ソートキーを基に倉庫在庫ソート
             * ソート後の倉庫在庫を戻り値に渡す
             */

            // 全倉庫在庫取得
            inZaikoViewModel.SokoZaikos = _context.SokoZaiko
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(shi => shi.ShohinMaster)
                .ToList();

            // 倉庫在庫絞り込み
            if (inZaikoViewModel.SetCodesList != null)
            {
                inZaikoViewModel.SokoZaikos = zaiko.NarrowSokoZaiko(inZaikoViewModel.SetCodesList);
            }

            // 倉庫在庫ソート
            inZaikoViewModel.SokoZaikos = zaiko.SortSokoZaiko(
                inZaikoViewModel.SokoZaikos, inZaikoViewModel.KeyEventDataList, inZaikoViewModel.DescendingFlagList);

            // null回避(修正予定)
            IList<int> numList = new List<int>();
            for (int i = 0; i < inZaikoViewModel.TableList.Count; i++)
            {
                int num = inZaikoViewModel.SelectCodeList.Where(x => x.Item2 == i).Count();
                numList.Add(num);
            }
            inZaikoViewModel.LabelNumList = numList;

            // ソート後の倉庫在庫を戻り値に渡す
            return inZaikoViewModel;
        }
    }
}
