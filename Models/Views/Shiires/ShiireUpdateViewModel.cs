using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Views.Shiires
{
    // 仕入実績、倉庫在庫更新用ViewModel
    public class ShiireUpdateViewModel
    {
        public IList<ShiireJisseki> ShiireJissekis { get; set; }

        public IList<SokoZaiko> SokoZaikos { get; set; }

        public bool? IsNormal { get; set; }

        public string? Remark { get; set; }
    }
}
