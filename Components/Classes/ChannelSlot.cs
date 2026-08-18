using ImageCombinerChannelExtractor.Components.Enums;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class ChannelSlot
    {
        public BitmapImage? Bitmap { get; set; }
        public (int Width, int Height) BitmapSize { get; set; } = (0, 0);

        public ChannelFilteringMode FilteringMode { get; set; } = ChannelFilteringMode.Bicubic;

        public string FilePath { get; set; } = string.Empty;
    }
}
