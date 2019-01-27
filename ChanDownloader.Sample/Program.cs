using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ChanDownloader.Console
{
    class Program
    {
        private static Downloader downloader = new Downloader();
        private static Thread _thread;

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
            var url = PromptUrl();
            while (string.IsNullOrEmpty(url)) url = PromptUrl();
            System.Console.Write("Path (may not exist; leave empty for working directory): ");
            var path = System.Console.ReadLine();

            System.Console.WriteLine("> loading thread");
            _thread = await downloader.LoadThread(url);

            if(_thread == null)
            {
                Utils.Log("Error loading thread");
                System.Console.WriteLine("> error loading thread");
                System.Console.ReadKey(true);
                return;
            }

            System.Console.Title = $"Chan Downloader - {_thread.Subject}";

            var files = _thread.Files;
            for (int i = 0; i < files.Count; i++) System.Console.WriteLine($"* {files[i].OriginalFileName} ({Math.Round(files[i].FileSize / 1024.0 / 1024.0, 2)}MiB)");
            System.Console.WriteLine($"{Environment.NewLine}> found {files.Count} files {Environment.NewLine}");

            System.Console.WriteLine("Press any key to begin download");
            System.Console.ReadKey(true);

            if (path.Equals(string.Empty)) path = $"{Directory.GetCurrentDirectory()}\\{_thread.SemanticSubject}";
            Directory.CreateDirectory(path);

            downloader.WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            await downloader.DownloadFiles(path);

            System.Console.WriteLine($"{Environment.NewLine}Press any key to continue");
            System.Console.ReadKey(true);
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Console.Write($"\r> downloaded {downloader.CurrentFileNumber} / {_thread.Files.Count}");
        }

        private static string PromptUrl()
        {
            System.Console.Write("Thread: ");
            return System.Console.ReadLine();
        }
    }

    public static class Utils
    {
        public static void Log(string text)
        {
            Debug.WriteLine($"ChanDownloader-log: {text}");
        }
    }
}
