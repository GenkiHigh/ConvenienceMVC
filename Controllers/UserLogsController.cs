using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.UserLogs;
using ConvenienceMVC.Models.Properties.Login;
using ConvenienceMVC.Models.Views.UserLogs;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConvenienceMVC.Controllers
{
    public class UserLogsController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        private IUser User;

        public UserLogsController(ConvenienceMVCContext context)
        {
            _context = context;
            User = new User(_context);
        }

        // Index(初期設定)
        public IActionResult Index(string inPageName)
        {
            KeepPageNamet(inPageName);
            return RedirectToAction("Login");
        }
        // ログイン
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel inUserLoginViewModel)
        {
            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            ModelState.Clear();

            // 入力されたメアドでDB検索
            // 見つからない場合再入力
            // パスワードが違う場合再入力
            UserLog queriedUserLog = User.QueryUserLog(inUserLoginViewModel.MailAddress);

            // 見つからない場合
            if (queriedUserLog == null)
            {
                return View();
            }
            else
            {
                // パスワードが違う場合
                if (queriedUserLog.Password != inUserLoginViewModel.Password)
                {
                    return View();
                }
            }

            // セッション追加
            AddSession(queriedUserLog);

            return RedirectToAction("Index", GetPageName());
        }

        // アカウント作成
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel inUserCreateViewModel)
        {
            // 入力不具合チェック
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            ModelState.Clear();

            // 入力されたメールアドレスを基にDB検索
            // そのメールアドレスで既に登録されている場合再入力
            // 入力したパスワード2種が違っていた場合再入力
            UserLog queriedUserLog = User.QueryUserLog(inUserCreateViewModel.MailAddress);

            // 見つかった場合
            if (queriedUserLog != null)
            {
                return View();
            }
            else
            {
                // パスワードが違った場合
                if (inUserCreateViewModel.Password != inUserCreateViewModel.RePassword)
                {
                    return View();
                }
            }

            // ユーザーIDを設定
            // 一番大きい数字に+1して渡す
            string userId = "";
            if (_context.UserLog.Count() == 0)
            {
                int firstNumber = 1;
                userId = firstNumber.ToString("D12");
            }
            else
            {
                userId = _context.UserLog.OrderBy(log => log.UserId).Select(log => log.UserId).Last();
                userId = (int.Parse(userId) + 1).ToString("D12");
            }

            // アカウント作成、DB追加
            UserLog newUserLog = new UserLog()
            {
                MailAddress = inUserCreateViewModel.MailAddress,
                UserId = userId,
                UserName = inUserCreateViewModel.UserName,
                Password = inUserCreateViewModel.Password,
                LastLoginDate = DateTime.Now,
            };
            _context.UserLog.Add(newUserLog);
            await _context.SaveChangesAsync();

            AddSession(newUserLog);

            return RedirectToAction("Index", GetPageName());
        }

        private void AddSession(UserLog inUserLog)
        {
            // セッション追加
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            HttpContext.Session.SetString("MyUserLog", JsonConvert.SerializeObject(inUserLog, settings));
        }

        // ページ名保存
        private void KeepPageNamet(string inPageName)
        {
            TempData["PageName"] = inPageName;
        }
        // ページ名復元
        private string GetPageName()
        {
            string PageName = TempData["PageName"] as string;
            return PageName;
        }
    }
}
