using ConvenienceMVC.Models.Interfaces.Zaikos;
using ConvenienceMVC.Models.Properties.Zaikos;
using ConvenienceMVC.Models.Views.Zaikos;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

namespace ConvenienceMVC.Models.Services.Zaikos
{
    public class ZaikoService : IZaikoService
    {
        private readonly ConvenienceMVCContext _context;
        private IZaiko zaiko;

        public ZaikoService(ConvenienceMVCContext context)
        {
            _context = context;
            zaiko = new Zaiko();
        }

        public ZaikoViewModel SortSokoZaiko(ZaikoViewModel inZaikoViewModel)
        {
            inZaikoViewModel.SokoZaikos = _context.SokoZaiko.Include(soko => soko.ShiireMaster).ThenInclude(shi => shi.ShohinMaster).ToList();

            inZaikoViewModel.SokoZaikos = zaiko.SortSokoZaiko(
                inZaikoViewModel.SokoZaikos, inZaikoViewModel.KeyEventData, inZaikoViewModel.DescendingFlag);

            return inZaikoViewModel;
        }
    }
}
