using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class FileSelectedEventArgs : RoutedEventArgs
    {
        public string? SelectedFilePath { get; }

        public FileSelectedEventArgs(RoutedEvent routedEvent, object source, string? filePath = null) : base(routedEvent, source)
        {
            SelectedFilePath = filePath;
        }
    }
}
