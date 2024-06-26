﻿namespace ConvenienceMVC.Models.Views.UserLogs
{
    // アカウント作成用ViewModel
    public class UserCreateViewModel
    {
        public string UserName { get; set; }

        public string MailAddress { get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }

        public bool? IsCreate { get; set; }

        public string? Remark { get; set; }
    }
}
