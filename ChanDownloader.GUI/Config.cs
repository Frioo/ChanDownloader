using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            public const string DownloadQueue = "Download queue";
            public static List<string> ActionList = new List<string>(new string[] { Fetch, Download, FetchAndDownload, DownloadQueue });
        }

        public static class Queue
        {
            public static ObservableCollection<QueueItem> Items = new ObservableCollection<QueueItem>();
        }
    }
}
