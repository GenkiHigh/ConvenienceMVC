using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace ConvenienceMVC.Models.Views.Zaikos
{
    public class ZaikoSearchViewModel
    {
        [DisplayName("ソートキー")]
        public IList<string?>? KeyEventDataList { get; set; }

        [DisplayName("□昇順 / ■降順")]
        public IList<bool>? DescendingFlagList { get; set; }

        [DisplayName("絞り込み(or)")]
        public IList<string>? SetCodesList { get; set; }

        public IList<int>? LabelPunctuationList { get; set; }

        public IList<SokoZaiko>? SokoZaikos { get; set; }

        public IList<string> TableList = new List<string>()
        {
            "仕入先コード",
            "商品コード"
        };

        public IList<(string, int)> SelectCodeList = new List<(string, int)>()
        {
            ("SS001",0),
            ("SS002",0),
            ("SS003",0),
            ("SS004",0),
            ("SS005",0),
            ("パプリカカレー",1),
            ("DXファイアフライザー",1),
            ("Hey！落ち着け！",1),
            ("分割されたハッピー",1),
        };

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
                new SelectListItem { Value = nameof(ChumonJissekiMeisai.ChumonZan), Text = "注文残" },
            }, "Value", "Text");
    }
}
