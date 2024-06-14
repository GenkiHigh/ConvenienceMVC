using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.UserLogs;
using ConvenienceMVC_Context;

namespace ConvenienceMVC.Models.Properties.Login
{
    public class User : IUser
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        public UserLog UserLog { get; set; }

        // コンストラクタ
        public User(ConvenienceMVCContext context)
        {
            _context = context;
        }

        // ユーザー情報検索
        public UserLog UserLogQuery(string inMail)
        {
            UserLog queriedUserLog = _context.UserLog
                .Where(log => log.MailAddress == inMail)
                .FirstOrDefault();

            UserLog = queriedUserLog;

            return UserLog;
        }
    }
}
