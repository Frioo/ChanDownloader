using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChanDownloader
{
    public class Downloader
    {
        public WebClient WebClient = new WebClient();
        private Thread _thread;

        public async Task LoadThread(string url)
        {
            var doc = new HtmlDocument();
            Utils.Log("LoadThread: downloading html");
            doc.LoadHtml(await WebClient.DownloadStringTaskAsync(url));

            Utils.Log("LoadThread: extracting info");
            var subject = doc.DocumentNode
                .SelectSingleNode("//head/title").InnerText;

            var id = doc.DocumentNode
                .SelectSingleNode("//div[contains(@class, 'postInfo')]/input[contains(@type, 'checkbox')]").Name;

            List<File> files = doc.DocumentNode
                .SelectNodes("//div[contains(@class,'fileText')]/a")
                .Select(node => new File(node.InnerText, node.ParentNode.Id, node.GetAttributeValue("href", string.Empty)))
                .Where(file => !file.Url.Equals(string.Empty))
                .ToList();

            this._thread = new Thread(url, id, subject, files, doc);
            Utils.Log($"Loaded thread: {id} - {subject} ({_thread.Files.Count} files)");
        }

        public List<File> GetFileList()
        {
            return _thread.Files;
        }

        public string GetThreadTitle()
        {
            return _thread.Subject;
        }

        public async Task DownloadFiles(string path)
        {
            var files = GetFileList();
            for (int i = 0; i < files.Count; i++)
            {
                var current = files[i];
                var filename = $"{path}\\{current.OriginalFileName}";
                var uri = $"http:{current.Url}";
                Utils.Log($"downloading {uri} to {filename}");
                await WebClient.DownloadFileTaskAsync(uri, filename);
            }
            Utils.Log($"downloaded {files.Count} files to {path}");
        }
    }

    public class Thread
    {
        public string Url { get; private set; }
        public string Id { get; private set; }
        public string Subject { get; private set; }
        public List<File> Files { get; private set; }
        public HtmlDocument HtmlDocument { get; private set; }

        public Thread(string url, string id, string subject, List<File> files, HtmlDocument doc)
        {
            this.Url = url;
            Id = id;
            Subject = subject;
            Files = files;
            HtmlDocument = doc;
        }
    }

    public class File
    {
        public string OriginalFileName { get; private set; }
        public string FileName { get; private set; }
        public string Url { get; private set; }

        public File(string originalName, string filename, string url)
        {
            this.OriginalFileName = originalName;
            this.FileName = filename;
            this.Url = url;
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
