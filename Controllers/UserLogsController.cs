using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.UserLogs;
using ConvenienceMVC.Models.Services.UserLogs;
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
        // ユーザーサービスインターフェース
        private readonly IUserService UserService;

        // コンストラクタ
        public UserLogsController(ConvenienceMVCContext context)
        {
            // DBコンテキスト設定
            _context = context;
            // ユーザーサービスインターフェース初期化
            UserService = new UserService(_context);
        }

        // Index(初期設定)
        public IActionResult Index(string inPageName)
        {
            // 直前のページ名を保存
            KeepPageNamet(inPageName);

            // ログインページに移動
            return RedirectToAction("Login");
        }
        // ログイン
        public IActionResult Login()
        {
            // ログインページ表示
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
            // フォーム入力値リセット
            ModelState.Clear();

            // 機能１：入力された情報を基にログイン実行
            UserLog queriedUserLog = UserService.UserLogin(inUserLoginViewModel);
            // 機能１－１：対象のアカウントが見つからかった、又はパスワードが違った場合
            if (queriedUserLog == null)
            {
                // 再入力
                return View();
            }

            // セッションにユーザー情報を追加
            AddSession(queriedUserLog);

            // ログインページに移動する前のページに移動
            return RedirectToAction("Index", GetPageName());
        }

        // アカウント作成
        public IActionResult Create()
        {
            // アカウント作成画面に移動
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
            // フォーム入力値リセット
            ModelState.Clear();

            // 機能１：アカウント新規作成
            UserLog newUserLog = await UserService.CreateAcount(inUserCreateViewModel);
            // 機能１－１：既に入力されたメールアドレスが使われている、又は入力されたパスワード2種が違った場合
            if (newUserLog == null)
            {
                // 再入力
                return View();
            }

            // セッションにユーザー情報を追加
            AddSession(newUserLog);

            // アカウント作成ページに移動する前のページに移動
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
