﻿using ConvenienceMVC.Models.Views.Chumons;

namespace ConvenienceMVC.Models.Interfaces.Chumons
{
    // 注文サービスインターフェース
    public interface IChumonService
    {
        // 注文インターフェース
        public IChumon Chumon { get; set; }

        // 注文実績設定
        public Task<ChumonUpdateViewModel> ChumonSetting(string inShiireSakiId, DateOnly inChumonDate);
        // 注文実績更新
        public Task<ChumonUpdateViewModel> ChumonCommit(ChumonUpdateViewModel inChumonViewModel);
    }
}
