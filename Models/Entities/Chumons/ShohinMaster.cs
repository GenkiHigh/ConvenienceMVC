using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.Chumons
{
    [Table("shohin_master")]
    [PrimaryKey(nameof(ShohinId))]
    public class ShohinMaster
    {
        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShohinId { get; set; }

        [Column("shohin_name")]
        [DisplayName("商品名称")]
        [MaxLength(50)]
        [Required]
        public string ShohinName { get; set; }

        [Column("shohi_zairitu")]
        [DisplayName("消費税率")]
        [Precision(15, 2)]
        [Required]
        public decimal ShohinZeiritu { get; set; }

        [Column("shohi_zairitu_gaishoku")]
        [DisplayName("消費税率（外食）")]
        [Precision(15, 2)]
        [Required]
        public decimal ShohinZeirituGaishoku { get; set; }
    }
}
