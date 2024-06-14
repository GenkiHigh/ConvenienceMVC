using ConvenienceMVC.Models.Entities.Chumons;

namespace ConvenienceMVC.Models.Views.Chumons
{
    // 注文実績更新用ViewModel
    public class ChumonUpdateViewModel
    {
        public ChumonJisseki ChumonJisseki { get; set; }

        public bool? IsNormal { get; set; }

        public string? Remark { get; set; } = string.Empty;
    }
}
