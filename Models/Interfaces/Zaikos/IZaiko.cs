using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Zaikos
{
    public interface IZaiko
    {
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, IList<string?> inSortCodes, IList<bool> inDescendingFlags);
    }
}
