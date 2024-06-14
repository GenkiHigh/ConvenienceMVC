using ConvenienceMVC.Models.Entities.Chumons;

namespace ConvenienceMVC.Models.Interfaces.Chumons
{
    // 注文インターフェース
    public interface IChumon
    {
        // 注文実績
        public ChumonJisseki ChumonJisseki { get; set; }

        // 注文実績検索
        public ChumonJisseki ChumonQuery(string inShiireSakiCode, DateOnly inChumonDate);
        // 注文実績作成
        public ChumonJisseki ChumonCreate(string inShiireSakiCode, DateOnly inChumonDate);
        // 注文実績更新
        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki);
    }
}
