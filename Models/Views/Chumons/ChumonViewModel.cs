using ConvenienceMVC.Models.Entities.Chumons;

namespace ConvenienceMVC.Models.Views.Chumons
{
    public class ChumonViewModel
    {
        public ChumonJisseki ChumonJisseki { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;
    }
}
