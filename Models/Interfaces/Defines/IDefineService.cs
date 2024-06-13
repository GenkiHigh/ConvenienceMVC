using ConvenienceMVC.Models.Entities.UserLogs;

namespace ConvenienceMVC.Models.Interfaces.Defines
{
    public interface IDefineService
    {
        public UserLog IsUserSession();

        public void DeleteUserSession();
    }
}
