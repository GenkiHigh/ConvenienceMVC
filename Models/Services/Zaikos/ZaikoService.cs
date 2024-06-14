using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC.Models.Properties.Zaikos;
using ConvenienceMVC.Models.Views.Zaikos;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Zaikos
{
    // 在庫サービス
    public class ZaikoService : IZaikoService
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 在庫インターフェース
        public IZaiko Zaiko;

        // コンストラクタ
        public ZaikoService(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
            // 在庫インターフェースのインスタンス生成
            Zaiko = new Zaiko(_context);
        }

        // 倉庫在庫設定
        public ZaikoSearchViewModel ZaikoSetting()
        {
            // 新規倉庫在庫を設定する
            ZaikoSearchViewModel newZaikoSearchViewModel = new ZaikoSearchViewModel();

            // 倉庫在庫を設定する
            IList<SokoZaiko> queriedSokoZaikos = _context.SokoZaiko.AsNoTracking()
                .OrderBy(soko => soko.ShiirePrdId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(shi => shi.ShohinMaster)
                .ToList();

            // ソートキーリスト設定、降順フラグリスト設定
            IList<string?> keyList = new List<string?>();
            IList<bool> flagList = new List<bool>();
            for (int i = 0; i < 3; i++)
            {
                keyList.Add(null);
                flagList.Add(false);
            }

            // 区切りリスト設定
            IList<int> punctuationList = new List<int>();
            for (int i = 0; i < newZaikoSearchViewModel.TableList.Count; i++)
            {
                int num = newZaikoSearchViewModel.SelectCodeList.Where(x => x.Item2 == i).Count();
                punctuationList.Add(num);
            }

            // 検索用ViewModelを設定
            ZaikoSearchViewModel getZaikoSearchViewModel = new ZaikoSearchViewModel()
            {
                KeyEventDataList = keyList,
                DescendingFlagList = flagList,
                SokoZaikos = queriedSokoZaikos,
                SetCodesList = new List<string>(),
                LabelPunctuationList = punctuationList,
            };

            // 検索用ViewModelを渡す
            return getZaikoSearchViewModel;
        }

        // 表示倉庫在庫設定
        // inZaikoViewModel：ソートキー、絞り込みキーを格納したViewModel
        public ZaikoSearchViewModel DisplayZaikoSetting(ZaikoSearchViewModel inZaikoSearchViewModel)
        {
            // 全倉庫在庫を取得する
            ZaikoSearchViewModel getZaikoSearchViewModel = inZaikoSearchViewModel;
            getZaikoSearchViewModel.SokoZaikos = _context.SokoZaiko
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(shi => shi.ShohinMaster)
                .ToList();

            // 倉庫在庫を絞り込む
            if (getZaikoSearchViewModel.SetCodesList != null)
            {
                getZaikoSearchViewModel.SokoZaikos = Zaiko.NarrowSokoZaiko(getZaikoSearchViewModel.SetCodesList);
            }

            // 倉庫在庫をソートする
            getZaikoSearchViewModel.SokoZaikos = Zaiko.SortSokoZaiko(
                getZaikoSearchViewModel.SokoZaikos, getZaikoSearchViewModel.KeyEventDataList, getZaikoSearchViewModel.DescendingFlagList);
            ZaikoSearchViewModel sortZaikoSearchViewModel = getZaikoSearchViewModel;

            // 区切りリスト設定
            IList<int> punctuationList = new List<int>();
            for (int i = 0; i < sortZaikoSearchViewModel.TableList.Count; i++)
            {
                int num = sortZaikoSearchViewModel.SelectCodeList.Where(x => x.Item2 == i).Count();
                punctuationList.Add(num);
            }
            sortZaikoSearchViewModel.LabelPunctuationList = punctuationList;

            // ソート後の倉庫在庫を戻り値に渡す
            return sortZaikoSearchViewModel;
        }
    }
}
