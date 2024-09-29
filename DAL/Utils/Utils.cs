using System;

namespace DAL.Utils
{
    public class Utils
    {
        public static double HitungCicilanBulanan(decimal jumlahPinjaman, decimal sukuBungaTahunan, int jangkaWaktuBulan)
        {
            double sukuBungaBulanan = ((double)sukuBungaTahunan * 100) / 12 / 100;

            double pinjaman = (double)jumlahPinjaman;

            double cicilanBulanan = pinjaman * (sukuBungaBulanan / (1 - Math.Pow(1 + sukuBungaBulanan, -jangkaWaktuBulan)));

            return cicilanBulanan;
        }
    }
}
