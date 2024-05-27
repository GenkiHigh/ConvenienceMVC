using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Views.Shiires
{
    // 仕入実績更新用ViewModel
    public class ShiireViewModel
    {
        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        public IList<SokoZaiko> SokoZaikos { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; }
    }
}
