using ConvenienceMVC.Models.Entities.Shiires;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace ConvenienceMVC.Models.Views.Zaikos
{
    public class ZaikoViewModel
    {
        [DisplayName("ソートキー")]
        public IList<string?>? KeyEventDataList { get; set; }

        [DisplayName("□昇順 / ■降順")]
        public IList<bool> DescendingFlagList { get; set; }

        public IList<SokoZaiko> SokoZaikos { get; set; }

        public SelectList KeyList = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem { Value = nameof(SokoZaiko.ShiireSakiId), Text = "仕入先コード" },
                new SelectListItem { Value = nameof(SokoZaiko.ShiirePrdId), Text = "仕入商品コード" },
                new SelectListItem { Value = nameof(SokoZaiko.ShohinId), Text = "商品コード" },
                new SelectListItem { Value = nameof(SokoZaiko.ShiireMaster.ShohinMaster.ShohinName), Text = "商品名" },
                new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoCaseSu), Text = "在庫数" },
                new SelectListItem { Value = nameof(SokoZaiko.SokoZaikoSu), Text = "倉庫在庫数" },
                new SelectListItem { Value = nameof(SokoZaiko.LastShiireDate), Text = "直近仕入日" },
                new SelectListItem { Value = nameof(SokoZaiko.LastDeliveryDate), Text = "直近払出日" },
            }, "Value", "Text");
    }
}
