using ConvenienceMVC.Models.Views.Shiires;

namespace ConvenienceMVC.Models.Interfaces.Shiires
{
    public interface IShiireService
    {
        public IShiire Shiire { get; set; }

        public ShiireViewModel ShiireSetting(string inChumonId);
        public ShiireViewModel ShiireCommit(ShiireViewModel inShiireViewModel);
    }
}
