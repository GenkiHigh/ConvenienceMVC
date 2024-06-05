using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Zaikos
{
    public interface IZaiko
    {
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, string inSortCode, bool inDescendingFlag);
    }
}
