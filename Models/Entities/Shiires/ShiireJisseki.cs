﻿using ConvenienceMVC.Models.Entities.Chumons;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.Shiires
{
    [Table("shiire_jisseki")]
    [PrimaryKey(nameof(ChumonId), nameof(ShiireDate), nameof(SeqByShiireDate), nameof(ShiireSakiId), nameof(ShiirePrdId))]
    public class ShiireJisseki
    {
        [Column("chumon_code")]
        [DisplayName("注文コード")]
        [Required]
        public string ChumonId { get; set; }

        [Column("shiire_date")]
        [DisplayName("仕入日付")]
        [Required]
        public DateOnly ShiireDate { get; set; }

        [Column("seq_by_shiiredate")]
        [DisplayName("仕入SEQ")]
        [Required]
        public uint SeqByShiireDate { get; set; }

        [Column("shiire_datetime")]
        [DisplayName("仕入日時")]
        [Required]
        public DateTime ShiireDateTime { get; set; }

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

        [Column("nonyu_su")]
        [DisplayName("納入数")]
        [Precision(10, 2)]
        [Required]
        public decimal NonyuSu { get; set; }

        [Timestamp]
        public uint Version { get; set; }

        [ForeignKey(nameof(ChumonId) + "," + nameof(ShiireSakiId) + "," + nameof(ShiirePrdId) + "," + nameof(ShohinId))]
        public virtual ChumonJissekiMeisai? ChumonJissekiMeisai { get; set; }
    }
}
