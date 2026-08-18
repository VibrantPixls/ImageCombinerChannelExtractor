using System.Windows;

namespace ImageCombinerChannelExtractor
{
    public partial class App : Application
    {
        public static MainWindow MainWindowReference => (MainWindow)Current.MainWindow;
    }
}
