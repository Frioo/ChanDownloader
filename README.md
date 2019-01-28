# ChanDownloader
### 4chan image downloader  
Uses the [official 4chan API](https://github.com/4chan/4chan-API)  
Grab the latest releases [here](https://github.com/Frioo/ChanDownloader/releases/latest)

## Chan Downloader - API 
### Usage
```csharp
var downloader = new Downloader();          // create new ChanDownloader instance
var thread = await downloader.LoadThread(<thread_url>);  // load thread
var subject = thread.Subject;  // get thread subject (safe format)
var files = thread.Files;       // get the file list

/*
 * the webclient is exposed so you can hook up your event callbacks
 * you can get the current downloaded file index from downloader.CurrentFileNumber
*/
downloader.WebClient.DownloadFileCompleted += <your_callback>;
await downloader.DownloadFiles(<your_path>);  // download the files to the specified directory
```

## Chan Downloader - Console
Simple console program for downloading all images from a specified thread  

## Chan Downloader - GUI
*work in progress*
