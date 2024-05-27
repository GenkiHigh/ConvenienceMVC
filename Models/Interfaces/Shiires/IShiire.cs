using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Views.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Shiires
{
    // 仕入インターフェース
    public interface IShiire
    {
        // 仕入実績リスト
        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        // 倉庫在庫リスト
        public IList<SokoZaiko> SokoZaikos { get; set; }

        // 仕入実績問い合わせ
        public IList<ShiireJisseki> ShiireToiawase(string inChumonId);
        // 仕入実績作成
        public IList<ShiireJisseki> ShiireSakusei(string inChumonId);
        // 仕入実績更新
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis);

        // 倉庫在庫問い合わせ
        public IList<SokoZaiko> ZaikoToiawase(string inShiireSakiId);
        // 倉庫在庫作成
        public IList<SokoZaiko> ZaikoSakusei(string inShiireSakiId);
        // 倉庫在庫更新
        public IList<SokoZaiko> ZaikoUpdate(IList<SokoZaiko> inSokoZaikos);

        // 注文実績明細問い合わせ
        public bool ChumonJissekiMeisaiToiawase(string inChumonId);
        // 注文残倉庫在庫数増減
        public ShiireViewModel ChumonZanBalance(ShiireViewModel inShiireViewModel);
    }
}
