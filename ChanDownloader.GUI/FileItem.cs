using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.FileSize = $"{Math.Round(file.FileSize / 1024.0 / 1024.0, 2)} MiB";
            this.Selected = true;
        }
    }
}
