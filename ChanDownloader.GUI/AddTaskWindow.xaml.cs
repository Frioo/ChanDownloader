using MahApps.Metro.SimpleChildWindow;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;

namespace ChanDownloader.GUI
{
    /// <summary>
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : ChildWindow
    {
        public AddTaskWindow()
        {
            this.InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close(new List<string>(TextBoxUrls.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
