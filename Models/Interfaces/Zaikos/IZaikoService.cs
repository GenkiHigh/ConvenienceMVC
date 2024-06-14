using ConvenienceMVC.Models.Views.Zaikos;

namespace ConvenienceMVC.Models.Interfaces.Zaikos
{
    public interface IZaikoService
    {
        // 倉庫在庫設定
        public ZaikoSearchViewModel ZaikoSetting();

        // 表示倉庫在庫設定
        public ZaikoSearchViewModel DisplayZaikoSetting(ZaikoSearchViewModel inZaikoSearchViewModel);
    }
}
