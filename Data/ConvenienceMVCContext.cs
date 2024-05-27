using ConvenienceMVC.Models.Entities.Chumons;
using ConvenienceMVC.Models.Entities.Shiires;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC_Context
{
    public class ConvenienceMVCContext : DbContext
    {
        public ConvenienceMVCContext(DbContextOptions<ConvenienceMVCContext> options)
            : base(options)
        {
        }

        // 注文
        public DbSet<ShohinMaster> ShohinMaster { get; set; } = default!;
        public DbSet<ShiireSakiMaster> ShiireSakiMaster { get; set; } = default!;
        public DbSet<ShiireMaster> ShiireMaster { get; set; } = default!;
        public DbSet<ChumonJissekiMeisai> ChumonJissekiMeisai { get; set; } = default!;
        public DbSet<ChumonJisseki> ChumonJisseki { get; set; } = default!;

        // 仕入
        public DbSet<ShiireJisseki> ShiireJisseki { get; set; } = default!;
        public DbSet<SokoZaiko> SokoZaiko { get; set; } = default!;
    }
}
