using ConvenienceMVC.Models.Views.Chumons;

namespace ConvenienceMVC.Models.Interfaces.Chumons
{
    // 注文サービス
    public interface IChumonService
    {
        public IChumon Chumon { get; set; }

        public ChumonViewModel ChumonSetting(string inShiireSakiId, DateOnly inChumonDate);
        public ChumonViewModel ChumonCommit(ChumonViewModel inChumonViewModel);
    }
}
