using ConvenienceMVC.Models.Entities.UserLogs;

namespace ConvenienceMVC.Models.Interfaces.UserLogs
{
    // ユーザーインターフェース
    public interface IUser
    {
        // ユーザー情報
        public UserLog UserLog { get; set; }

        public UserLog UserLogQuery(string mail);
    }
}
