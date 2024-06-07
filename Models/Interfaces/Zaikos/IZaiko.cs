using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Zaikos
{
    public interface IZaiko
    {
        // 倉庫在庫絞り込み
        public IList<SokoZaiko> NarrowSokoZaiko(IList<string>? inNarrowCode);

        // 倉庫在庫並び替え
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, IList<string?> inSortCodes, IList<bool> inDescendingFlags);
    }
}
