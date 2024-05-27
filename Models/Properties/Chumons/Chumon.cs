using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Interfaces.Chumons;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Chumons
{
    // 注文プロパティ
    public class Chumon : IChumon
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 注文実績
        public ChumonJisseki ChumonJisseki { get; set; }

        // コンストラクタ
        public Chumon(ConvenienceMVCContext context)
        {
            // DBコンテキスト設定
            _context = context;
        }

        /*
         * 注文実績作成
         * inShiireSakiCode：選択された仕入先コード
         * inChumonDate：選択された注文日
         */
        public ChumonJisseki ChumonSakusei(string inShiireSakiCode, DateOnly inChumonDate)
        {
            /*
             * 注文実績を検索された注文日で新規作成する
             * 検索された注文日で注文IDを発番
             * 注文実績を新規作成
             * 注文実績明細を作成するために対応する仕入マスタを取得
             * 空の注文実績明細を作成
             * 空の注文実績及び明細を戻り値に渡す
             */

            // 注文ID発番(注文日基準)
            string chumonId = ChumonIDCreate(inChumonDate);

            // 注文実績新規作成
            ChumonJisseki = new ChumonJisseki()
            {
                ChumonId = chumonId,
                ShiireSakiId = inShiireSakiCode,
                ChumonDate = inChumonDate,
            };

            // 仕入マスタ取得
            var shiireMastars = _context.ShiireMaster
                .Where(sm => sm.ShiireSakiId == inShiireSakiCode)
                .Include(sm => sm.ShiireSakiMaster)
                .Include(sm => sm.ShohinMaster)
                .OrderBy(sm => sm.ShohinId);

            // 空の注文実績明細作成、新規作成した注文実績に追加
            ChumonJisseki.ChumonJissekiMeisais = new List<ChumonJissekiMeisai>();
            foreach (var shiire in shiireMastars)
            {
                ChumonJisseki.ChumonJissekiMeisais.Add(new ChumonJissekiMeisai()
                {
                    ChumonId = chumonId,
                    ShiireSakiId = inShiireSakiCode,
                    ShiirePrdId = shiire.ShiirePrdId,
                    ShohinId = shiire.ShohinId,
                    ChumonSu = 0,
                    ChumonZan = 0,
                    ShiireMaster = shiire,
                });
            }

            // 新規作成した注文実績及び明細を渡す
            return ChumonJisseki;
        }

        /*
         * 注文実績問い合わせ
         * inShiireSakiCode：選択された仕入先コード
         * inChumonDate：選択された注文日
         */
        public ChumonJisseki ChumonToiawase(string inShiireSakiCode, DateOnly inChumonDate)
        {
            /*
             * 注文実績検索画面で入力されたデータを元に注文実績がDBにあるかを判定する
             * ある場合は対象の注文実績を戻り値に渡す
             * 無い場合はnullを渡す
             */

            // 注文履歴が残っているかを判断
            ChumonJisseki = _context.ChumonJisseki
                .Where(ch => ch.ShiireSakiId == inShiireSakiCode && ch.ChumonDate == inChumonDate)
                .Include(chm => chm.ChumonJissekiMeisais)
                .ThenInclude(shi => shi.ShiireMaster)
                .ThenInclude(sho => sho.ShohinMaster)
                .FirstOrDefault();

            // ある場合は対象の注文実績を、無い場合はnullを渡す
            return ChumonJisseki;
        }

        /*
         * 注文実績更新
         * inChumonJisseki：入力された注文数が格納されている注文実績更新用ViewModel
         */
        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki)
        {
            /*
             * 初期表示されていた内容と入力後の内容が変わっているかを判定
             * 変わっていない場合初期表示されていた値を戻り値に渡す
             * 変わっている場合、今回の注文実績が既存データの更新か新規作成かを判定
             * 更新の場合、既存データの注文数残を変動させる
             * 新規作成の場合、注文実績及び明細をDBに追加する
             * 更新後又は追加後の注文実績を戻り値に渡す
             */

            // 初期表示されていた内容と入力後の内容が変わっているかを判定
            bool changeFlag = false;
            for (int i = 0; i < ChumonJisseki.ChumonJissekiMeisais.Count; i++)
            {
                // 注文数が変更されていたら更新処理に移動
                if (ChumonJisseki.ChumonJissekiMeisais[i].ChumonSu != inChumonJisseki.ChumonJissekiMeisais[i].ChumonSu)
                {
                    changeFlag = true;
                    break;
                }
            }

            // 更新処理
            if (changeFlag)
            {
                // その注文実績が既にある場合更新、新規の場合追加
                var chumonJisseki = ChumonJisseki;
                chumonJisseki = _context.ChumonJisseki
                    .Where(chu => chu.ChumonId == inChumonJisseki.ChumonId && chu.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                    .Include(chu => chu.ChumonJissekiMeisais)
                    .FirstOrDefault();

                // DBにデータがある場合
                if (chumonJisseki != null)
                {
                    // 注文数残を更新
                    for (int i = 0; i < ChumonJisseki.ChumonJissekiMeisais.Count; i++)
                    {
                        // 初期表示の値と入力後の値の差分、注文数残を変動
                        decimal chumonSa = inChumonJisseki.ChumonJissekiMeisais[i].ChumonSu - chumonJisseki.ChumonJissekiMeisais[i].ChumonSu;
                        chumonJisseki.ChumonJissekiMeisais[i].ChumonSu += chumonSa;
                        chumonJisseki.ChumonJissekiMeisais[i].ChumonZan += chumonSa;

                        // 注文残が0未満にならないよう調整
                        if (chumonJisseki.ChumonJissekiMeisais[i].ChumonZan < 0)
                        {
                            chumonJisseki.ChumonJissekiMeisais[i].ChumonSu -= chumonJisseki.ChumonJissekiMeisais[i].ChumonZan;
                            chumonJisseki.ChumonJissekiMeisais[i].ChumonZan = 0;
                        }
                    }

                    // 注文実績を更新
                    ChumonJisseki = chumonJisseki;
                }
                // 追加
                else
                {
                    // 注文実績を追加
                    _context.ChumonJisseki.Add(inChumonJisseki);

                    // 注文実績明細を追加
                    foreach (var item in inChumonJisseki.ChumonJissekiMeisais)
                    {
                        _context.ChumonJissekiMeisai.Add(item);
                    }

                    // 注文実績を更新
                    ChumonJisseki = inChumonJisseki;
                }
            }
            else
            {
                // 初期表示と内容が同じでありDBにデータがない場合、データ追加
                var chumonJisseki = ChumonJisseki;
                chumonJisseki = _context.ChumonJisseki
                    .Where(chu => chu.ChumonId == inChumonJisseki.ChumonId && chu.ShiireSakiId == inChumonJisseki.ShiireSakiId)
                    .Include(chu => chu.ChumonJissekiMeisais)
                    .FirstOrDefault();
                // DBにデータが無い場合
                if (chumonJisseki == null)
                {
                    // 注文実績を追加
                    _context.ChumonJisseki.Add(inChumonJisseki);

                    // 注文実績明細を追加
                    foreach (var item in inChumonJisseki.ChumonJissekiMeisais)
                    {
                        _context.ChumonJissekiMeisai.Add(item);
                    }

                    // 注文実績を更新
                    ChumonJisseki = inChumonJisseki;
                }
            }

            // 注文実績を渡す
            return ChumonJisseki;
        }

        /*
         * 注文ID発番
         * inChumonDate：選択された注文日
         */
        public string ChumonIDCreate(DateOnly inChumonDate)
        {
            /*
             * 選択された注文日に応じて適切なIDを発番
             * 注文実績が0の場合、選択された注文日で新規作成(エラー回避)
             * 注文実績テーブルを注文IDで昇順ソートし、一番最後の値を注文IDとして取得
             * 選択された注文日を文字列で取得、今日の日付を文字列で取得、注文IDの後ろ三桁を文字列で取得
             * 選択した注文日でその日初めての注文の場合注文番号を001に、注文済みの場合注文番号を1追加
             * "注文年月日-三桁の数字"の形の文字列を戻り値に渡す(その日の注文実績数が1000以上の場合はnullを渡す)
             */

            // エラー回避
            if (_context.ChumonJisseki.Count() == 0)
            {
                string newDay = inChumonDate.ToString("yyyyMMdd-");
                string newNum = "001";
                return newDay + newNum;
            }

            // 注文ID、選択注文日、実績内注文日、注文番号を設定
            string chumonId = _context.ChumonJisseki
                .OrderBy(x => x.ChumonId).Select(x => x.ChumonId).Last();
            string chumonDate = inChumonDate.ToString("yyyyMMdd-");
            string getChumonDate = chumonId.Substring(0, 9);
            string num = chumonId.Substring(chumonId.Length - 3);

            // 入力した注文日で初めての注文の場合注文番号を001に、既に注文済みなら番号を1追加
            num = chumonDate == getChumonDate ? (uint.Parse(num) + 1).ToString("D3") : "001";

            // "注文年月日-注文番号三桁"を渡す(その日の注文実績数が1000以上の場合nullを渡す)
            return uint.Parse(num) >= 999 ? null : chumonDate + num;
        }
    }
}
