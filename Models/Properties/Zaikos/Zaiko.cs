using ConvenienceMVC.Models.Entities.Shiires;
using ConvenienceMVC.Models.Interfaces.Zaikos;

namespace ConvenienceMVC.Models.Properties.Zaikos
{
    public class Zaiko : IZaiko
    {
        public IList<SokoZaiko> SortSokoZaiko(IList<SokoZaiko> inSokoZaikos, string inSortCode, bool inDescendingFlag)
        {
            // 昇順
            if (!inDescendingFlag)
            {
                switch (inSortCode)
                {
                    case "ShiireSakiId":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.ShiireSakiId).ToList();
                        break;

                    case "ShiirePrdId":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.ShiirePrdId).ToList();
                        break;

                    case "ShohinId":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.ShohinId).ToList();
                        break;

                    case "ShohinName":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.ShiireMaster.ShohinMaster.ShohinName).ToList();
                        break;

                    case "SokoZaikoCaseSu":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.SokoZaikoCaseSu).ToList();
                        break;

                    case "SokoZaikoSu":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.SokoZaikoSu).ToList();
                        break;

                    case "LastShiireDate":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.LastShiireDate).ToList();
                        break;

                    case "LastDeliveryDate":
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.LastDeliveryDate).ToList();
                        break;

                    default:
                        inSokoZaikos = inSokoZaikos.OrderBy(soko => soko.ShiireSakiId).ToList();
                        break;
                }
            }
            // 降順
            else
            {
                switch (inSortCode)
                {
                    case "ShiireSakiId":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.ShiireSakiId).ToList();
                        break;

                    case "ShiirePrdId":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.ShiirePrdId).ToList();
                        break;

                    case "ShohinId":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.ShohinId).ToList();
                        break;

                    case "ShohinName":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.ShiireMaster.ShohinMaster.ShohinName).ToList();
                        break;

                    case "SokoZaikoCaseSu":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.SokoZaikoCaseSu).ToList();
                        break;

                    case "SokoZaikoSu":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.SokoZaikoSu).ToList();
                        break;

                    case "LastShiireDate":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.LastShiireDate).ToList();
                        break;

                    case "LastDeliveryDate":
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.LastDeliveryDate).ToList();
                        break;

                    default:
                        inSokoZaikos = inSokoZaikos.OrderByDescending(soko => soko.ShiireSakiId).ToList();
                        break;
                }
            }

            return inSokoZaikos;
        }
    }
}
