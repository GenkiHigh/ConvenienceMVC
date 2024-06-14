using ConvenienceMVC.Models.Entities.UserLogs;

namespace ConvenienceMVC.Models.Interfaces.Defines
{
    // 基底サービスインターフェース
    public interface IDefineService
    {
        // セッション内ユーザー情報確認
        public UserLog IsUserSession();

        // セッション内ユーザー情報削除
        public void DeleteUserSession();
    }
}
