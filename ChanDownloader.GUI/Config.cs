using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanDownloader.GUI
{
    public class Config
    {
        public static class Actions
        {
            public const string Fetch = "Fetch";
            public const string Download = "Download";
            public const string FetchAndDownload = "Fetch and Download";
            public static List<string> ActionList = new List<string>(new string[] { Fetch, Download, FetchAndDownload });
        }
    }
}
