using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvenienceMVC.Models.Entities.UserLogs
{
    [Table("user_login")]
    [PrimaryKey(nameof(UserId))]
    public class UserLog
    {
        [Column("user_id")]
        [DisplayName("ユーザーID")]
        [MaxLength(20)]
        [Required]
        public string UserId { get; set; }

        [Column("mail_adress")]
        [DisplayName("メールアドレス")]
        [MaxLength(20)]
        [Required]
        public string MailAddress { get; set; }

        [Column("user_name")]
        [DisplayName("ユーザー名")]
        [MaxLength(20)]
        [Required]
        public string UserName { get; set; }

        [Column("password")]
        [DisplayName("パスワード")]
        [MaxLength(20)]
        [Required]
        public string Password { get; set; }

        [Column("last_login_date")]
        [DisplayName("最終ログイン日")]
        public DateTime LastLoginDate { get; set; }
    }
}
