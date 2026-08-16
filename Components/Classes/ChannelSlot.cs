using ImageCombinerChannelExtractor.Components.Enums;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class ChannelSlot
    {
        public BitmapImage? Bitmap { get; set; }
        public ChannelFilteringMode FilteringMode { get; set; } = ChannelFilteringMode.Bicubic;

        public string FilePath { get; set; } = string.Empty;
    }
}
