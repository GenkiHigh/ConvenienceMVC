using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Views.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Shiires
{
    public interface IShiire
    {
        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        public IList<SokoZaiko> SokoZaikos { get; set; }

        public IList<ShiireJisseki> ShiireToiawase(string inChumonId);
        public IList<ShiireJisseki> ShiireSakusei(string inChumonId);
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis);
        public ShiireViewModel ChumonZanBalance(ShiireViewModel inShiireViewModel);
        public bool ChumonJissekiMeisaiToiawase(string inChumonId);
        public IList<SokoZaiko> ZaikoToiawase(string inShiireSakiId);
        public IList<SokoZaiko> ZaikoSakusei(string inShiireSakiId);
        public IList<SokoZaiko> ZaikoUpdate(IList<SokoZaiko> inSokoZaikos);
    }
}
