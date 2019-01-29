using System;

namespace ChanDownloader.GUI
{
    public static class Utils
    {
        public static double Mibibytes(int size)
        {
            return Math.Round(size / 1024.0 / 1024.0, 2);
        }
    }
}
