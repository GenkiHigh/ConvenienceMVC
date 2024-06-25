using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Shiires;
using ConvenienceMVC.Models.Views.Shiires;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Properties.Shiires
{
    // 仕入
    public class Shiire : IShiire
    {
        // DBコンテキスト
        private readonly ConvenienceMVCContext _context;

        // 仕入実績リスト
        public IList<ShiireJisseki> ShiireJissekis { get; set; }

        // 倉庫在庫リスト
        public IList<SokoZaiko> SokoZaikos { get; set; }

        // コンストラクタ
        public Shiire(ConvenienceMVCContext context)
        {
            // DBコンテキストを設定
            _context = context;
        }

        // 仕入実績検索
        // inChumonId：検索画面で選択された注文コード
        public async Task<IList<ShiireJisseki>> ShiireQuery(string inChumonId)
        {
            // 処理１：仕入実績を検索する
            // 戻り値：検索結果(ある場合は対応する仕入実績リスト、無い場合はnull)

            // 処理１：選択された注文コードに対応する仕入実績リストを検索する
            IList<ShiireJisseki> queriedShiireJissekis = await _context.ShiireJisseki
                .Where(sj => sj.ChumonId == inChumonId)
                .Include(sj => sj.ChumonJissekiMeisai)
                .ThenInclude(mei => mei.ChumonJisseki)
                .ThenInclude(chu => chu.ShiireSakiMaster)
                .ThenInclude(ss => ss.ShiireMasters)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToListAsync();
            // 処理１－１：検索結果を仕入実績に設定する
            ShiireJissekis = queriedShiireJissekis;

            // 取得した仕入実績リストを渡す(無い場合はnullを渡す)
            return ShiireJissekis;
        }

        // 仕入実績作成
        // inChumonId：検索画面で選択された注文コード
        public async Task<IList<ShiireJisseki>> ShiireCreate(string inChumonId)
        {
            // 処理１：新規仕入実績内のデータを設定する
            // 処理２：新規仕入実績リストを作成する
            // 戻り値：作成した仕入実績リスト

            // 処理１：新規仕入実績内のデータを設定する
            // 仕入日時
            DateTime shiireDateTime = DateTime.Now;
            // 仕入日付
            DateOnly shiireDate = DateOnly.FromDateTime(shiireDateTime);
            // 仕入SEQ
            uint seqByShiireDate;
            // 仕入先コード
            string shiireSakiId;
            // 仕入商品コード
            IList<string> shiirePrdIds;
            // 商品コード
            IList<string> shohinIds;
            // 納入数
            decimal nonyuSu = 0;

            // 仕入SEQを設定する
            // DBの仕入実績にデータが一件も無い場合は1、ある場合は最大仕入SEQに1追加する
            seqByShiireDate = await _context.ShiireJisseki.CountAsync() == 0 ? 1 :
                await _context.ShiireJisseki.Select(sj => sj.SeqByShiireDate).MaxAsync() + 1;

            // 選択された注文コードに対応する注文実績明細を取得する
            IQueryable<ChumonJissekiMeisai> meisais = _context.ChumonJissekiMeisai.Where(mei => mei.ChumonId == inChumonId);

            // 仕入先コードリストを設定する
            shiireSakiId = meisais.Select(mei => mei.ShiireSakiId).First();
            //仕入商品コードリストを設定する
            shiirePrdIds = meisais.Select(mei => mei.ShiirePrdId).ToList();
            // 商品コードリストを設定する
            shohinIds = meisais.Select(mei => mei.ShohinId).ToList();

            // インクルード済み注文実績明細リストを作成する
            IList<ChumonJissekiMeisai> includeMeisais = await _context.ChumonJissekiMeisai
                .Where(mei => mei.ChumonId == inChumonId)
                .OrderBy(mei => mei.ShohinId)
                .Include(mei => mei.ChumonJisseki)
                .ThenInclude(chu => chu.ShiireSakiMaster)
                .ThenInclude(sak => sak.ShiireMasters)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToListAsync();
            // ここまで処理１

            // 処理２：新規仕入実績リストを作成する
            IList<ShiireJisseki> newShiireJissekis = new List<ShiireJisseki>();
            for (int shiiresCounter = 0; shiiresCounter < meisais.Count(); shiiresCounter++)
            {
                // 新規仕入実績を追加する
                newShiireJissekis.Add(new ShiireJisseki()
                {
                    ChumonId = inChumonId,
                    ShiireDate = shiireDate,
                    SeqByShiireDate = seqByShiireDate,
                    ShiireDateTime = shiireDateTime,
                    ShiireSakiId = shiireSakiId,
                    ShiirePrdId = shiirePrdIds[shiiresCounter],
                    ShohinId = shohinIds[shiiresCounter],
                    NonyuSu = nonyuSu,
                    ChumonJissekiMeisai = includeMeisais[shiiresCounter],
                });
            }
            // 処理２－１：新規作成した仕入実績リストを仕入実績リストに設定する
            ShiireJissekis = newShiireJissekis;

            // 作成した仕入実績リストを渡す
            return ShiireJissekis;
        }

        // 仕入実績更新
        // inShiireJissekis：更新画面での入力後の仕入実績リスト
        public async Task<IList<ShiireJisseki>> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis)
        {
            // 処理１：仕入実績を検索する
            // 処理２A：仕入実績を更新する
            // 処理２B：仕入実績を追加する
            // 処理３：仕入実績を設定する
            // 戻り値：更新後、又は追加後の仕入実績

            // 処理１：仕入実績を検索する
            IList<ShiireJisseki>? isShiireJissekis = await _context.ShiireJisseki
                .Where(sj => sj.ShiireSakiId == inShiireJissekis.First().ShiireSakiId &&
                sj.ChumonId == inShiireJissekis.First().ChumonId)
                .OrderBy(sj => sj.ShohinId)
                .ToListAsync();
            // 処理１－１：見つかった場合
            if (isShiireJissekis.Count != 0)
            {
                // 楽観的排他制御
                uint current = _context.Entry(inShiireJissekis[0]).Property(v => v.Version).CurrentValue;
                uint current2 = _context.Entry(isShiireJissekis[0]).Property(v => v.Version).CurrentValue;
                if (current != current2)
                {
                    throw new Exception("既に更新されています。");
                }
                // 仕入者が違う場合アベンド
                if (isShiireJissekis[0].UserId != inShiireJissekis[0].UserId)
                {
                    throw new Exception("仕入者が違います。");
                }

                // 処理２A：入力前の納入数を入力後の納入数と同じにする
                for (int shiiresCounter = 0; shiiresCounter < isShiireJissekis.Count; shiiresCounter++)
                {
                    isShiireJissekis[shiiresCounter].NonyuSu = inShiireJissekis[shiiresCounter].NonyuSu;
                }
                // 処理２A-1：仕入実績を更新する
                foreach (ShiireJisseki shiire in isShiireJissekis)
                {
                    _context.ShiireJisseki.Update(shiire);
                }

                // 処理３:仕入実績を設定する
                ShiireJissekis = isShiireJissekis;
            }
            // 処理１－２：見つからなかった場合
            else
            {
                // 対応する注文実績が無い場合アベンド
                ChumonJisseki queriedChumonJisseki = await _context.ChumonJisseki
                    .Where(x => x.ChumonId == inShiireJissekis[0].ChumonId)
                    .FirstOrDefaultAsync();

                if (queriedChumonJisseki == null)
                {
                    throw new Exception("注文実績が存在しません。");
                }
                // 注文者と仕入者が同じ場合アベンド
                if (queriedChumonJisseki.UserId == inShiireJissekis[0].UserId)
                {
                    throw new Exception("注文者と仕入者が同じです。");
                }

                // 新規追加時の入力値が全て0の場合、追加しない
                bool isAdd = false;
                for (int shiiresCounter = 0; shiiresCounter < inShiireJissekis.Count; shiiresCounter++)
                {
                    if (inShiireJissekis[shiiresCounter].NonyuSu != 0)
                    {
                        isAdd = true;
                        break;
                    }
                }
                if (isAdd)
                {
                    // 処理２B：仕入実績を追加する
                    for (int shiiresCounter = 0; shiiresCounter < inShiireJissekis.Count; shiiresCounter++)
                    {
                        inShiireJissekis[shiiresCounter].ShiireDateTime =
                            DateTime.SpecifyKind(inShiireJissekis[shiiresCounter].ShiireDateTime, DateTimeKind.Utc);
                        _context.ShiireJisseki.Add(inShiireJissekis[shiiresCounter]);
                    }
                }

                // 処理３：仕入実績を設定する
                ShiireJissekis = inShiireJissekis;
            }

            // 更新後、又は追加後の仕入実績を渡す
            return ShiireJissekis;
        }

        // 倉庫在庫検索
        // inShiireSakiId：選択された仕入実績の仕入先コード
        public async Task<IList<SokoZaiko>> ZaikoQuery(string inShiireSakiId)
        {
            // 処理１：倉庫在庫を検索する
            // 戻り値：検索結果(ある場合は対応する倉庫在庫リスト、無い場合はnull)

            // 処理１：倉庫在庫を検索する
            IList<SokoZaiko> querySokoZaikos = await _context.SokoZaiko
                .Where(soko => soko.ShiireSakiId == inShiireSakiId)
                .Include(soko => soko.ShiireMaster)
                .ThenInclude(sm => sm.ShohinMaster)
                .ToListAsync();
            // 処理１－１：検索結果を倉庫在庫に設定する
            SokoZaikos = querySokoZaikos;

            // 対象の倉庫在庫を渡す(無い場合はnullを渡す)
            return SokoZaikos;
        }

        // 倉庫在庫作成
        // inShiireSakiId：選択された仕入実績の仕入先コード
        public async Task<IList<SokoZaiko>> ZaikoCreate(string inShiireSakiId)
        {
            // 処理１：仕入マスタを設定する
            IList<ShiireMaster> queriedShiireMastars = await _context.ShiireMaster
                .Where(sm => sm.ShiireSakiId == inShiireSakiId)
                .ToListAsync();
            // 処理１－１：新規倉庫在庫を作成する
            IList<SokoZaiko> newSokoZaikos = new List<SokoZaiko>();
            foreach (ShiireMaster shiire in queriedShiireMastars)
            {
                SokoZaiko soko = new SokoZaiko()
                {
                    ShiireSakiId = inShiireSakiId,
                    ShiirePrdId = shiire.ShiirePrdId,
                    ShohinId = shiire.ShohinId,
                    SokoZaikoCaseSu = 0,
                    SokoZaikoSu = 0,
                    LastShiireDate = DateOnly.FromDateTime(DateTime.Today),
                    LastDeliveryDate = null,
                };
                newSokoZaikos.Add(soko);
            }
            // 処理１－２：新規倉庫在庫リストを倉庫在庫リストに設定する
            SokoZaikos = newSokoZaikos;

            // 空の倉庫在庫リストを渡す
            return SokoZaikos;
        }

        // 倉庫在庫更新
        // inSokoZaikos：更新画面での入力後の倉庫在庫リスト
        public async Task<IList<SokoZaiko>> ZaikoUpdate(IList<SokoZaiko> inSokoZaikos)
        {
            // 処理１：倉庫在庫を検索する
            // 処理２A：倉庫在庫を更新する
            // 処理２B：倉庫在庫を追加する
            // 戻り値：更新後、又は追加後の倉庫在庫

            // 処理１：倉庫在庫を検索する
            IList<SokoZaiko>? isSokoZaikos = await _context.SokoZaiko
                .Where(sz => sz.ShiireSakiId == inSokoZaikos.First().ShiireSakiId)
                .OrderBy(sz => sz.ShohinId)
                .ToListAsync();
            // 処理１－１：見つかった場合
            if (isSokoZaikos.Count != 0)
            {
                // 楽観的排他制御
                uint current = _context.Entry(inSokoZaikos[0]).Property(v => v.Version).CurrentValue;
                uint current2 = _context.Entry(isSokoZaikos[0]).Property(v => v.Version).CurrentValue;
                if (current != current2)
                {
                    throw new Exception("既に更新されています。");
                }

                // 処理２A：入力前の倉庫在庫数を入力後と同じにする
                for (int zaikosCounter = 0; zaikosCounter < isSokoZaikos.Count; zaikosCounter++)
                {
                    isSokoZaikos[zaikosCounter].SokoZaikoSu = inSokoZaikos[zaikosCounter].SokoZaikoSu;
                    isSokoZaikos[zaikosCounter].SokoZaikoCaseSu =
                        Math.Floor(inSokoZaikos[zaikosCounter].SokoZaikoSu / isSokoZaikos[zaikosCounter].ShiireMaster.ShiirePcsPerUnit);
                    isSokoZaikos[zaikosCounter].LastDeliveryDate = DateOnly.FromDateTime(DateTime.Today);
                }
                // 処理２Aー１：倉庫在庫を更新する
                foreach (SokoZaiko zaiko in isSokoZaikos)
                {
                    _context.SokoZaiko.Update(zaiko);
                }
                // 処理２Aー２：更新後の倉庫在庫を倉庫在庫に設定する
                SokoZaikos = isSokoZaikos;
            }
            // 処理１－２：見つからなかった場合
            else
            {
                // 新規追加時、入力値が全て0の場合、追加しない
                bool isAdd = false;
                for (int zaikosCounter = 0; zaikosCounter < inSokoZaikos.Count; zaikosCounter++)
                {
                    if (inSokoZaikos[zaikosCounter].SokoZaikoSu != 0)
                    {
                        isAdd = true;
                        break;
                    }
                }
                if (isAdd)
                {
                    // 処理２B：倉庫在庫を追加する
                    for (int zaikosCounter = 0; zaikosCounter < inSokoZaikos.Count; zaikosCounter++)
                    {
                        inSokoZaikos[zaikosCounter].SokoZaikoCaseSu =
                            Math.Floor(inSokoZaikos[zaikosCounter].SokoZaikoSu / inSokoZaikos[zaikosCounter].ShiireMaster.ShiirePcsPerUnit);
                        inSokoZaikos[zaikosCounter].LastDeliveryDate = DateOnly.FromDateTime(DateTime.Today);

                        _context.SokoZaiko.Add(inSokoZaikos[zaikosCounter]);
                    }
                }
                // 処理２Bー１：追加後の倉庫在庫を倉庫在庫に設定する
                SokoZaikos = inSokoZaikos;
            }

            // 更新後、又は追加後の倉庫在庫リストを渡す
            return SokoZaikos;
        }

        // 注文実績明細検索
        // inChumonId：検索画面で選択された注文コード
        public async Task<ChumonJissekiMeisai?> ChumonJissekiMeisaiQuery(string inChumonId)
        {
            // 処理１：注文実績明細を検索する
            // 戻り値：検索結果(ある場合は対応する注文実績明細、無い場合はnull)

            // 処理１：注文実績明細を検索する
            ChumonJissekiMeisai? queriedChumonJissekiMeisai = await _context.ChumonJissekiMeisai.AsNoTracking()
                .Where(mei => mei.ChumonId == inChumonId)
                .FirstOrDefaultAsync();

            // 検索結果を渡す(無い場合はnullを渡す)
            return queriedChumonJissekiMeisai;
        }

        // 注文残、倉庫在庫数変動
        // inShiireUpdateViewModel：更新画面での入力前の仕入実績、倉庫在庫更新用ViewModel
        public async Task<ShiireUpdateViewModel> ChumonZanBalance(ShiireUpdateViewModel inShiireUpdateViewModel)
        {
            // 処理１：必要な変数を設定する
            // 処理２：注文残、倉庫在庫数を変動する
            // 戻り値：変動後の仕入実績、倉庫在庫を格納した更新用ViewModel

            // 処理１：注文実績明細を取得する
            IList<ChumonJissekiMeisai> queriedMeisais = await _context.ChumonJissekiMeisai
                .Where(mei => mei.ShiireSakiId == inShiireUpdateViewModel.ShiireJissekis[0].ShiireSakiId &&
                mei.ChumonId == inShiireUpdateViewModel.ShiireJissekis[0].ChumonId)
                .OrderBy(mei => mei.ShohinId)
                .ToListAsync();
            // 処理１ー１：変更後仕入実績リストを設定する
            IList<ShiireJisseki> transShiireJissekis = new List<ShiireJisseki>();
            // 処理１－２：変更後倉庫在庫リストを設定する
            IList<SokoZaiko> transSokoZaikos = new List<SokoZaiko>();

            // 処理２：注文残、倉庫在庫数を変動する
            for (int shiiresCounter = 0; shiiresCounter < inShiireUpdateViewModel.ShiireJissekis.Count; shiiresCounter++)
            {
                // 処理２－１：仕入実績、倉庫在庫を設定する
                transShiireJissekis.Add(inShiireUpdateViewModel.ShiireJissekis[shiiresCounter]);
                transSokoZaikos.Add(inShiireUpdateViewModel.SokoZaikos[shiiresCounter]);

                // 処理２－２：注文残以上の納入を制限する
                transShiireJissekis[shiiresCounter].NonyuSu =
                    inShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu - ShiireJissekis[shiiresCounter].NonyuSu
                    <= queriedMeisais[shiiresCounter].ChumonZan ?
                    inShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu :
                    ShiireJissekis[shiiresCounter].NonyuSu + queriedMeisais[shiiresCounter].ChumonZan;

                // 処理２－３：更新画面での入力前の納入数と比較して注文残、倉庫在庫数の変動量を設定する
                decimal transNum = inShiireUpdateViewModel.ShiireJissekis[shiiresCounter].NonyuSu - ShiireJissekis[shiiresCounter].NonyuSu;

                // 処理２－４：注文残、倉庫在庫数を変動する
                queriedMeisais[shiiresCounter].ChumonZan -= transNum;
                transSokoZaikos[shiiresCounter].SokoZaikoSu += transNum;

                // 処理２－５：注文実績明細を更新する(更新待機)(倉庫在庫は後に更新待機する)
                _context.ChumonJissekiMeisai.Update(queriedMeisais[shiiresCounter]);
            }

            // 変動後の仕入実績、倉庫在庫を格納した更新用ViewModelを作成し渡す
            return new ShiireUpdateViewModel()
            {
                ShiireJissekis = transShiireJissekis,
                SokoZaikos = transSokoZaikos,
            };
        }
    }
}
