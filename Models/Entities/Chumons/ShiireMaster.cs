using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.Chumons
{
    [Table("shiire_master")]
    [PrimaryKey(nameof(ShiireSakiId), nameof(ShiirePrdId), nameof(ShohinId))]
    public class ShiireMaster
    {
        [Column("shiire_saki_code")]
        [DisplayName("仕入先コード")]
        [MaxLength(10)]
        [Required]
        public string ShiireSakiId { get; set; }

        [Column("shiire_prd_code")]
        [DisplayName("仕入商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShiirePrdId { get; set; }

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        [MaxLength(10)]
        [Required]
        public string ShohinId { get; set; }

        [Column("shiire_prd_name")]
        [DisplayName("仕入商品名")]
        [MaxLength(30)]
        [Required]
        public string ShiirePrdName { get; set; }

        [Column("shiire_pcs_unit")]
        [DisplayName("仕入単位における数量")]
        [Precision(15, 2)]
        [Required]
        public decimal ShiirePcsPerUnit { get; set; }

        [Column("shiire_unit")]
        [DisplayName("仕入単位")]
        [MaxLength(10)]
        [Required]
        public string ShiireUnit { get; set; }

        [Column("shiire_tanka")]
        [DisplayName("仕入単価")]
        [Precision(15, 2)]
        [Required]
        public decimal ShiireTanka { get; set; }

        [ForeignKey(nameof(ShiireSakiId))]
        public virtual ShiireSakiMaster? ShiireSakiMaster { get; set; }

        [ForeignKey(nameof(ShohinId))]
        public virtual ShohinMaster? ShohinMaster { get; set; }
    }
}
