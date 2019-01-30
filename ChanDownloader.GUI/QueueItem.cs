namespace ChanDownloader.GUI
{
    public class QueueItem
    {
        public string ThreadUrl { get; private set; }
        public bool IsComplete { get; set; }

        public QueueItem(string url)
        {
            this.ThreadUrl = url;
            this.IsComplete = false;
        }
    }
}
