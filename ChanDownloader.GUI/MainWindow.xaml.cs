using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChanDownloader.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Downloader _downloader = new Downloader();
        private Thread _thread;
        private ObservableCollection<FileItem> _items;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChanDownloader.Utils.Log("MainWindow: loaded");
            Config.Queue.Items.CollectionChanged += Items_CollectionChanged;
            ButtonAction.Content = Config.Actions.Fetch;
            SetStatus("Ready");
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ChanDownloader.Utils.Log($"Queue updated: {e.Action}");
            if (Config.Queue.Items.Count > 1)
            {
                ButtonAction.IsEnabled = true;
                ButtonAction.Content = Config.Actions.DownloadQueue;
                SetStatus($"Queue ready: {Config.Queue.Items.Where(item => !item.IsComplete).Count()} threads");
            }
        }

        private async Task LoadThread(string url)
        {
            SetStatus("Loading thread...");
            ProgressRing.Visibility = Visibility.Visible;
            this._thread = await _downloader.LoadThread(url);

            if (_thread == null)
            {
                SetStatus("Could not load thread");
                ChanDownloader.Utils.Log("LoadThread: error loading thread");
                MessageBox.Show("Could not load thread", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _items = new ObservableCollection<FileItem>(_thread.Files.Select(file => new FileItem(file)));
            DataGridFiles.DataContext = _items;
            ButtonAction.Content = Config.Actions.Download;
            ProgressRing.Visibility = Visibility.Hidden;
            SetStatus($"Loaded thread: {_thread.Id} | {_thread.Files.Count} files - {Utils.Mibibytes(_thread.Files.Sum(file => file.FileSize))} MiB");
            SetTitle(_thread.Subject);
        }

        private async Task Download()
        {
            SetStatus("Downloading files...");
            ProgressRing.Visibility = Visibility.Visible;
            var path = $"{Directory.GetCurrentDirectory()}\\{_thread.SemanticSubject}";
            Directory.CreateDirectory(path);
            _downloader.WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            await _downloader.DownloadFiles(_items.Where(item => item.Selected).Select(item => item.File).ToList(), path);
        }

        private async Task DownloadQueue()
        {
            for (int i = 0; i < Config.Queue.Items.Count; i++)
            {
                try
                {
                    await LoadThread(Config.Queue.Items[i].ThreadUrl);
                    await Download();
                    Config.Queue.Items[i].IsComplete = true;
                    ChanDownloader.Utils.Log($"Queue item completed: {Config.Queue.Items[i].ThreadUrl}");
                }
                catch (Exception ex)
                {
                    ChanDownloader.Utils.Log($"Error downloading queue item {Config.Queue.Items[i].ThreadUrl} - {ex.Message}");
                    ResetState();
                    SetStatus($"Queue error: invalid URL? '{Config.Queue.Items[i].ThreadUrl}'");
                }
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var queueTotal = Config.Queue.Items.Count;
            var queueCompleted = Config.Queue.Items.Where(item => item.IsComplete).Count();

            SetStatus($"Thread {queueCompleted + 1} / {queueTotal} | Downloading files... {_downloader.CurrentFileNumber} / {_items.Count}");

            if (_downloader.CurrentFileNumber == _items.Count)
            {
                ProgressRing.Visibility = Visibility.Hidden;
                SetStatus($"Thread {queueCompleted + 1} / {queueTotal} | Downloaded {_items.Count} files");
            }

        }


        private void SetTitle(string text)
        {
            this.Title = $"Chan Downloader - {text}";
        }

        private void SetStatus(string text)
        {
            StatusBarItem.Content = text;
        }

        private void ResetState()
        {
            ProgressRing.Visibility = Visibility.Hidden;
            ButtonAction.IsEnabled = false;
            ButtonAction.Content = Config.Actions.Fetch;
        }


        private void CheckBoxAutoDownload_Checked(object sender, RoutedEventArgs e)
        {
            ButtonAction.Content = _thread == null ? Config.Actions.FetchAndDownload : Config.Actions.Download;
        }

        private void CheckBoxAutoDownload_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_thread == null) ButtonAction.Content = Config.Actions.Fetch;
            else ButtonAction.Content = Config.Actions.Download;
        }

        private async void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonAction.Content.ToString().Equals(Config.Actions.Fetch))
            {
                try
                {
                    Config.Queue.Items.Add(new QueueItem(TextBoxUrl.Text));

                    if (Config.Queue.Items.Count > 1)
                    {
                        await DownloadQueue();
                    }
                    else
                    {
                        await LoadThread(TextBoxUrl.Text);
                    }
                }
                catch (Exception ex)
                {
                    ChanDownloader.Utils.Log($"LoadThread call unsuccessful: {ex.Message}");
                    ResetState();
                    SetStatus("Could not load thread");
                    //MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ButtonAction.Content.ToString().Equals(Config.Actions.Download))
            {
                try
                {
                    await Download();
                }
                catch (Exception ex)
                {
                    ChanDownloader.Utils.Log($"Download unsuccessful: {ex.Message}");
                    ResetState();
                    SetStatus("Error downloading files");
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (ButtonAction.Content.ToString().Equals(Config.Actions.DownloadQueue))
            {
                if (Config.Queue.Items.Count != 0)
                {
                    await DownloadQueue();
                }
            }
            
        }

        private async void MenuAddTask_Click(object sender, RoutedEventArgs e)
        {
            var added = await this.ShowChildWindowAsync<List<QueueItem>>(new AddTaskWindow());
            if (added != null) added.ForEach(item => Config.Queue.Items.Add(item));
        }

        private void MenuClearQueue_Click(object sender, RoutedEventArgs e)
        {
            ResetState();
            SetStatus($"Removed {Config.Queue.Items.Count} threads from queue");
            Config.Queue.Items.Clear();
        }
    }
}
