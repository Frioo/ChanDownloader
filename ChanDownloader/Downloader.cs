using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChanDownloader
{
    public class Downloader
    {
        public WebClient WebClient = new WebClient();
        public int CurrentFileNumber = 1;
        private Thread _thread;
        private const string api_url = @"https://a.4cdn.org/";
        private const string api_img_url = @"https://i.4cdn.org/";

        public async Task LoadThread(string url)
        {
            Utils.Log("LoadThread: extracting info");
            //turn http://boards.4chan.org/<board>/thread/<id>/<sometimes_title> into <board>/thread/<id>
            var endpoint = string.Join("/", url.Remove(0, url.LastIndexOf('.')).Split('/'), 1, 3);
            Utils.Log($"api url: {api_url}{endpoint}");

            var posts = JObject.Parse(await WebClient.DownloadStringTaskAsync($"{api_url}{endpoint}.json"))["posts"].ToObject<JArray>();
            if (posts == null) return; // something went wrong ;)

            var id = posts[0]["no"].ToString();
            var subject = posts[0]["semantic_url"].ToString();
            var files = new List<File>();

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i]["filename"] != null)
                {
                    var filename = posts[i]["filename"].ToString() + posts[i]["ext"].ToString();
                    var renamed = posts[i]["tim"].ToString() + posts[i]["ext"].ToString();
                    var uri = $"{api_img_url}{endpoint.Split('/').First()}/{renamed}";
                    files.Add(new File(filename, renamed, uri));
                }
            }

            this._thread = new Thread(url, id, subject, files);
            //Utils.Log($"Loaded thread: {id} - {subject} ({_thread.Files.Count} files)");
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
                this.CurrentFileNumber = i + 1;
                var current = files[i];
                var filename = $"{path}\\{current.OriginalFileName}";
                var uri = $"{current.Url}";
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

        public Thread(string url, string id, string subject, List<File> files)
        {
            this.Url = url;
            Id = id;
            Subject = subject;
            Files = files;
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
