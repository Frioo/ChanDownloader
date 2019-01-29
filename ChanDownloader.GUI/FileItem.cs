namespace ChanDownloader.GUI
{
    public class FileItem
    {
        public bool Selected { get; set; }
        public File File { get; private set; }
        public string FileSize { get; private set; }

        public FileItem(File file)
        {
            this.File = file;
            this.FileSize = $"{Utils.Mibibytes(file.FileSize)} MiB";
            this.Selected = true;
        }
    }
}
