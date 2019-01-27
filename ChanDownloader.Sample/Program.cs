using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ChanDownloader.Sample
{
    class Program
    {
        private static Downloader downloader = new Downloader();

        static void Main(string[] args)
        {
            while(true)
            {
                Console.Title = "Chan Downloader";
                ShowMenu().GetAwaiter().GetResult();
            }
        }

        private static async Task ShowMenu()
        {
            Console.Clear();
            Console.Write("Thread: ");
            var url = Console.ReadLine();
            Console.Write("Path (may not exist; leave empty for working directory): ");
            var path = Console.ReadLine();

            Console.WriteLine("> loading thread");
            await downloader.LoadThread(url);

            Console.Title = $"Chan Downloader - {downloader.GetThreadTitle()}";

            var files = downloader.GetFileList();
            Console.WriteLine($"> found {files.Count} files {Environment.NewLine}");
            for (int i = 0; i < files.Count; i++) Console.WriteLine($"* {files[i].OriginalFileName}");

            Console.WriteLine($"{Environment.NewLine}Press any key to begin download");
            Console.ReadKey(true);

            if (path.Equals(string.Empty)) path = $"{Directory.GetCurrentDirectory()}\\{Utils.GetSafeFilename(downloader.GetThreadTitle())}";
            Directory.CreateDirectory(path);

            downloader.WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            await downloader.DownloadFiles(path);

            Console.WriteLine($"{Environment.NewLine}Press any key to continue");
            Console.ReadKey(true);
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.Write($"\r> downloaded {downloader.CurrentFileNumber} / {downloader.GetFileList().Count}");
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
