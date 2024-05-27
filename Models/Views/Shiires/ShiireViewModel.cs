using ConvenienceMVC.Models.Entities.Shiires;

namespace ConvenienceMVC.Models.Views.Shiires
{
    public class ShiireViewModel
    {
        public IList<ShiireJisseki> ShiireJissekis { get; set; }
        public IList<SokoZaiko> SokoZaikos { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; }
    }
}
