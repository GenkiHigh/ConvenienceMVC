using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.UserLogs;
using ConvenienceMVC.Models.Properties.Login;
using ConvenienceMVC.Models.Views.UserLogs;
using ConvenienceMVC_Context;

namespace ConvenienceMVC.Models.Services.UserLogs
{
    public class UserService : IUserService
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // ユーザーインターフェース
        public IUser User;

        // コンストラクタ
        public UserService(ConvenienceMVCContext context)
        {
            _context = context;
            User = new User(_context);
        }

        // ログイン
        public UserLog UserLogin(UserLoginViewModel inUserLoginViewModel)
        {
            // 機能１：入力されたメールアドレスでアカウント検索
            UserLog queriedUserLog = User.QueryUserLog(inUserLoginViewModel.MailAddress);
            // 機能１－１：対象のアカウントが見つからなかった場合
            if (queriedUserLog == null)
            {
                // 再入力
                return null;
            }
            // 機能１－２：対象のアカウントが見つかった場合
            else
            {
                // アカウントが設定しているパスワードと入力したパスワードが違った場合
                if (queriedUserLog.Password != inUserLoginViewModel.Password)
                {
                    // 再入力
                    return null;
                }
            }

            // アカウントが見つかり、パスワードも一致した場合、対象のアカウントを渡す
            return queriedUserLog;
        }

        // アカウント作成
        public async Task<UserLog> CreateAcount(UserCreateViewModel inUserCreateViewModel)
        {
            // 機能１：アカウント検索
            // 機能２：アカウント作成
            // 機能３：DB更新

            // 機能１：入力されたメールアドレスでアカウント検索
            UserLog queriedUserLog = User.QueryUserLog(inUserCreateViewModel.MailAddress);
            // 機能１－１：対象のアカウントが見つからなかった場合
            if (queriedUserLog == null)
            {
                // 設定したいパスワードと再入力したパスワードが違った場合
                if (inUserCreateViewModel.Password != inUserCreateViewModel.RePassword)
                {
                    // 再入力
                    return null;
                }
            }
            // 機能１－２：対象のアカウントが見つかった場合
            else
            {
                // 再入力
                return null;
            }

            // 機能２：ユーザーIDを設定
            string userId = "";
            // 機能２－１ー１：DBにアカウントが一件も登録されていない場合(エラー回避)
            if (_context.UserLog.Count() == 0)
            {
                // ユーザーIDを"000000000001"に設定する(12桁)
                int firstNumber = 1;
                userId = firstNumber.ToString("D12");
            }
            // 機能２－１ー２：DBにアカウントが一件以上登録されている場合
            else
            {
                // 登録されている一番大きいユーザーIDを取得
                userId = _context.UserLog.OrderBy(log => log.UserId).Select(log => log.UserId).Last();
                // 一番大きいユーザーIDに１追加して新たなユーザーIDを設定する(12桁)
                userId = (int.Parse(userId) + 1).ToString("D12");
            }
            // 機能２－２：アカウント新規作成
            UserLog newUserLog = new UserLog()
            {
                MailAddress = inUserCreateViewModel.MailAddress,
                UserId = userId,
                UserName = inUserCreateViewModel.UserName,
                Password = inUserCreateViewModel.Password,
                LastLoginDate = DateTime.Now,
            };

            // 機能３：DB更新
            _context.UserLog.Add(newUserLog);
            await _context.SaveChangesAsync();

            // 新規作成したアカウントを渡す
            return newUserLog;
        }
    }
}
