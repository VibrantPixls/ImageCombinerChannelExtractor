using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class EnumFriendlyNameHelper
    {
        public static string GetFriendlyName(ChannelFilteringMode value)
        {
            return value switch
            {
                ChannelFilteringMode.Bilinear => StringLinesInfo.EnumFriendlyNameChannelFilteringBilinear,
                ChannelFilteringMode.NearestNeighbor => StringLinesInfo.EnumFriendlyNameChannelFilteringNearestNeighbor,
                _ => StringLinesInfo.EnumFriendlyNameChannelFilteringBicubic
            };
        }
    }
}
