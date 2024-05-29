using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.Chumons
{
    [Table("chumon_jisseki_meisai")]
    [PrimaryKey(nameof(ChumonId), nameof(ShiireSakiId), nameof(ShiirePrdId), nameof(ShohinId))]
    public class ChumonJissekiMeisai
    {
        [Column("chumon_code")]
        [DisplayName("注文コード")]
        [MaxLength(20)]
        [Required]
        public string ChumonId { get; set; }

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

        [Column("chumon_su")]
        [DisplayName("注文数")]
        [Precision(10, 2)]
        [Required]
        public decimal ChumonSu { get; set; }

        [Column("chumon_zan")]
        [DisplayName("注文残")]
        [Precision(10, 2)]
        [Required]
        public decimal ChumonZan { get; set; }

        [Timestamp]
        public uint Version { get; set; }

        [ForeignKey(nameof(ChumonId) + "," + nameof(ShiireSakiId))]
        public virtual ChumonJisseki? ChumonJisseki { get; set; }

        [ForeignKey(nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        public virtual ShiireMaster? ShiireMaster { get; set; }
    }
}
