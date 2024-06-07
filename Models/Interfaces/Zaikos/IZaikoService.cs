using ConvenienceMVC.Models.Views.Zaikos;

namespace ConvenienceMVC.Models.Interfaces.Zaikos
{
    public interface IZaikoService
    {
        // 表示倉庫在庫設定
        public ZaikoViewModel SetDisplaySokoZaiko(ZaikoViewModel inZaikoViewModel);
    }
}
