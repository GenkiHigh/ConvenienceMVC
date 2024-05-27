using ConvenienceMVC.Models.Entities.Chumons;

namespace ConvenienceMVC.Models.Interfaces.Chumons
{
    public interface IChumon
    {
        public ChumonJisseki ChumonJisseki { get; set; }

        public ChumonJisseki ChumonSakusei(string inShiireSakiCode, DateOnly inChumonDate);
        public ChumonJisseki ChumonToiawase(string inShiireSakiCode, DateOnly inChumonDate);
        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki);
    }
}
