using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
