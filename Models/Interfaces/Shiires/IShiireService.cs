﻿using ConvenienceMVC.Models.Views.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Shiires
{
    // 仕入サービスインターフェース
    public interface IShiireService
    {
        // 仕入インターフェース
        public IShiire Shiire { get; set; }

        // 仕入実績、倉庫在庫設定
        public Task<ShiireUpdateViewModel> ShiireSetting(string inChumonId);

        // 仕入実績、倉庫在庫更新
        public Task<ShiireUpdateViewModel> ShiireCommit(ShiireUpdateViewModel inShiireViewModel);
    }
}
