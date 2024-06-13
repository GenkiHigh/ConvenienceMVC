using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Views.UserLogs;

namespace ConvenienceMVC.Models.Interfaces.UserLogs
{
    // ユーザーサービスインターフェース
    public interface IUserService
    {
        // ログイン
        public UserLog UserLogin(UserLoginViewModel inUserLoginViewModel);

        // アカウント作成
        public Task<UserLog> CreateAcount(UserCreateViewModel inUserCreateViewModel);
    }
}
