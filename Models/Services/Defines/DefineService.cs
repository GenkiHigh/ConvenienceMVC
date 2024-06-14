using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using Newtonsoft.Json;

namespace ConvenienceMVC.Models.Services.Defines
{
    public class DefineService : IDefineService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefineService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserLog IsUserSession()
        {
            // セッション情報取得
            var userSession = _httpContextAccessor.HttpContext.Session.GetString("MyUserLog");

            // セッション情報が無い場合
            if (userSession == null)
            {
                // ログインページに飛ぶ
                return null;
            }
            else
            {
                UserLog getUserLog = JsonConvert.DeserializeObject<UserLog>(userSession);
                return getUserLog;
            }
        }

        public void DeleteUserSession()
        {
            _httpContextAccessor.HttpContext.Session.Remove("MyUserLog");
        }
    }
}
