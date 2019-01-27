using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ChanDownloader.Console
{
    class Program
    {
        private static Downloader downloader = new Downloader();

        static void Main(string[] args)
        {
            while(true)
            {
                System.Console.Title = "Chan Downloader";
                ShowMenu().GetAwaiter().GetResult();
            }
        }

        private static async Task ShowMenu()
        {
            System.Console.Clear();
            System.Console.Write("Thread: ");
            var url = System.Console.ReadLine();
            System.Console.Write("Path (may not exist; leave empty for working directory): ");
            var path = System.Console.ReadLine();

            System.Console.WriteLine("> loading thread");
            await downloader.LoadThread(url);

            System.Console.Title = $"Chan Downloader - {downloader.GetThreadTitle()}";

            var files = downloader.GetFileList();
            System.Console.WriteLine($"> found {files.Count} files {Environment.NewLine}");
            for (int i = 0; i < files.Count; i++) System.Console.WriteLine($"* {files[i].OriginalFileName}");

            System.Console.WriteLine($"{Environment.NewLine}Press any key to begin download");
            System.Console.ReadKey(true);

            if (path.Equals(string.Empty)) path = $"{Directory.GetCurrentDirectory()}\\{Utils.GetSafeFilename(downloader.GetThreadTitle())}";
            Directory.CreateDirectory(path);

            downloader.WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            await downloader.DownloadFiles(path);

            System.Console.WriteLine($"{Environment.NewLine}Press any key to continue");
            System.Console.ReadKey(true);
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Console.Write($"\r> downloaded {downloader.CurrentFileNumber} / {downloader.GetFileList().Count}");
        }
    }

    public static class Utils
    {
        public static void Log(string text)
        {
            Debug.WriteLine($"ChanDownloader-log: {text}");
        }

        public static string GetSafeFilename(string filename)
        {

            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));

        }
    }
}
