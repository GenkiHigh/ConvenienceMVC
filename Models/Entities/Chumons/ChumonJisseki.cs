using ConvenienceMVC.Models.Entities.UserLogs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.Chumons
{
    [Table("chumon_jisseki")]
    [PrimaryKey(nameof(ChumonId), nameof(ShiireSakiId))]
    public class ChumonJisseki
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

        [Column("chumon_date")]
        [DisplayName("注文日")]
        public DateOnly? ChumonDate { get; set; }

        [Column("user_id")]
        [DisplayName("ユーザーID")]
        [MaxLength(20)]
        public string? UserId { get; set; }

        [Timestamp]
        public uint Version { get; set; }

        [ForeignKey(nameof(ShiireSakiId))]
        public virtual ShiireSakiMaster? ShiireSakiMaster { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserLog? UserLog { get; set; }

        public virtual IList<ChumonJissekiMeisai>? ChumonJissekiMeisais { get; set; } = new List<ChumonJissekiMeisai>() { };
    }
}
