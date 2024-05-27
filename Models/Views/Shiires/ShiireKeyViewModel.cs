using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Views.Shiires
{
    public class ShiireKeyViewModel
    {
        [Column("chumon_code")]
        [DisplayName("注文コード")]
        [Required]
        public string ChumonId { get; set; }

        public List<SelectListItem>? ChumonIdList { get; set; }
    }
}
