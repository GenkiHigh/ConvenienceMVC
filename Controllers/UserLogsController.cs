using ConvenienceMVC.Models.Entities.UserLogs;
using ConvenienceMVC.Models.Interfaces.Defines;
using ConvenienceMVC.Models.Interfaces.UserLogs;
using ConvenienceMVC.Models.Services.Defines;
using ConvenienceMVC.Models.Services.UserLogs;
using ConvenienceMVC.Models.Views.UserLogs;
using ConvenienceMVC_Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConvenienceMVC.Controllers
{
    // ユーザーコントローラー
    public class UserLogsController : Controller
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;
        // 基底サービスインターフェース
        private readonly IDefineService DefineService;
        // ユーザーサービスインターフェース
        private readonly IUserService UserService;

        // コンストラクタ
        public UserLogsController(ConvenienceMVCContext context, DefineService defineService)
        {
            // DBコンテキスト設定
            _context = context;
            // ユーザーサービスインターフェース初期化
            UserService = new UserService(_context);
            // 基底サービスインターフェース設定
            DefineService = defineService;
        }

        // Index(初期設定)
        public IActionResult Index(string inPageName)
        {
            // 直前のページ名を保存
            KeepPageNamet(inPageName);

            // ログインページに移動
            return RedirectToAction("Login");
        }
        // ログイン(初期設定)
        public IActionResult Login()
        {
            // ログインページ表示
            return View();
        }
        // ログイン(ログイン実行)
        // inUserLoginViewModel：ログイン画面で入力されたデータを格納したログイン用ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel inUserLoginViewModel)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：ログインを実行する
            // 戻り値：ログイン画面に移動する前の画面への移動、再入力

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            // 処理１－１：フォーム入力値をリセットする
            ModelState.Clear();

            // 処理２：ログイン実行
            UserLog queriedUserLog = UserService.UserLogin(inUserLoginViewModel);
            // 処理２－１：対象のアカウントが見つからかった、又はパスワードが違った場合
            if (queriedUserLog == null)
            {
                // 再入力
                return View();
            }
            // 処理２－２：対象のアカウントが見つかった場合
            else
            {
                // 処理２－２ー１：セッションにユーザー情報を追加する
                AddSession(queriedUserLog);
            }

            // ログイン画面に移動する前の画面に移動する
            return RedirectToAction("Index", GetPageName());
        }

        // アカウント作成(初期設定)
        public IActionResult Create()
        {
            // アカウント作成画面に移動
            return View();
        }
        // アカウント作成(作成実行)
        // inUserCreateViewModel：作成画面で入力されたデータを格納した作成用ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel inUserCreateViewModel)
        {
            // 処理１：入力に不具合があるかをチェックする
            // 処理２：アカウントを作成する
            // 戻り値：アカウント作成画面に移動する前の画面への移動、再入力

            // 処理１：入力に不具合があるかをチェックする
            if (!ModelState.IsValid)
            {
                throw new Exception("Postデータエラー");
            };
            // 処理１－１：フォーム入力値をリセットする
            ModelState.Clear();

            // 処理２：新規アカウントを作成する
            UserLog newUserLog = await UserService.CreateAcount(inUserCreateViewModel);
            // 処理２－１：既に入力されたメールアドレスが使われている、又は入力されたパスワード2種が違った場合
            if (newUserLog == null)
            {
                // 再入力
                return View();
            }
            // 処理２－２：メールアドレスが使われておらず、入力されたパスワード2種が合っていた場合
            else
            {
                // 処理２－２－１：セッションにユーザー情報を追加する
                AddSession(newUserLog);
            }

            // アカウント作成画面に移動する前の画面に移動
            return RedirectToAction("Index", GetPageName());
        }

        // ログアウト
        public IActionResult Logout()
        {
            // ユーザーセッション削除
            DefineService.DeleteUserSession();

            return RedirectToAction("Index", "Menus");
        }

        // セッション追加
        // inUserLog：追加したいユーザー情報
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
        // inPageName：保存したいページ名(ログイン、アカウント作成ページから戻るページ)
        private void KeepPageNamet(string inPageName)
        {
            TempData["PageName"] = inPageName;
        }
        // ページ名復元
        private string GetPageName()
        {
            // 戻り値：復元したページ名

            string PageName = TempData["PageName"] as string;
            return PageName;
        }
    }
}
