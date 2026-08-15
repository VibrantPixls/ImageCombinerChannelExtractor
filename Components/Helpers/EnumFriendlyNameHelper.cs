using ImageCombinerChannelExtractor.Components.Enums;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class EnumFriendlyNameHelper
    {
        public static string GetFriendlyName(ChannelFilteringMode value)
        {
            return value switch
            {
                ChannelFilteringMode.Bilinear => "Smooth (Bilinear)",
                ChannelFilteringMode.NearestNeighbor => "Sharp (Nearest Neighbor)",
                _ => "Smooth (Bicubic)"
            };
        }
    }
}
