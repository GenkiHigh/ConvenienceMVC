using ConvenienceMVC.Models.Entities.UserLogs;

namespace ConvenienceMVC.Models.Interfaces.UserLogs
{
    public interface IUser
    {
        public UserLog UserLog { get; set; }

        public UserLog QueryUserLog(string mail);
    }
}
