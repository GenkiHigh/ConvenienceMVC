using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using Newtonsoft.Json;

namespace ConvenienceMVC.Models.Services.Defines
{
    // 基底サービス
    public class DefineService : IDefineService
    {
        // Httpリクエストインターフェース
        private readonly IHttpContextAccessor _httpContextAccessor;

        // コンストラクタ
        public DefineService(IHttpContextAccessor httpContextAccessor)
        {
            // Httpリクエストインターフェースを設定
            _httpContextAccessor = httpContextAccessor;
        }

        // セッション内ユーザー情報確認
        public UserLog IsUserSession()
        {
            // 処理１：セッション内にユーザー情報があるかを確認する
            // 戻り値：取得したユーザー情報(無い場合はnull)

            // 処理１：セッションを取得する
            string? userSession = _httpContextAccessor.HttpContext.Session.GetString("MyUserLog");
            // 処理１－１：セッションが無い場合
            if (userSession == null)
            {
                // ユーザー情報が無いことを渡す
                return null;
            }
            // 処理１－２：セッションがある場合
            else
            {
                // 処理１－２－１：ユーザー情報を復元する
                UserLog getUserLog = JsonConvert.DeserializeObject<UserLog>(userSession);

                // 復元したユーザー情報を渡す
                return getUserLog;
            }
        }

        // セッション内ユーザー情報削除
        public void DeleteUserSession()
        {
            // 処理１：ユーザー情報を削除する

            // 処理１：ユーザー情報を削除する
            _httpContextAccessor.HttpContext.Session.Remove("MyUserLog");
        }
    }
}
