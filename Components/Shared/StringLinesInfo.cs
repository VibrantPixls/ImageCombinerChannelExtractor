using ImageCombinerChannelExtractor.Components.Enums;

namespace ImageCombinerChannelExtractor.Components.Shared
{
    public static class StringLinesInfo
    {
        public const string notificationCombining = "Started combining input images";
        public const string notificationSuccessfullCombining = "Successfully created combined image";
        public const string notificationSuccessfullCombiningExport = "Successfully exported combined image";

        public const string notificationExtracting = "Started extracting channels from input image";
        public const string notificationSuccessfullExtracting = "Successfully extracted channels from input image";

        public const string EnumFriendlyNameChannelFilteringBilinear = "Smooth (Bilinear)";
        public const string EnumFriendlyNameChannelFilteringNearestNeighbor = "Sharp (Nearest Neighbor)";
        public const string EnumFriendlyNameChannelFilteringBicubic = "Smooth (Bicubic)";

        public const string NoInputImageTextDefault = "No Image Selected";
        public static string GetClrChnBtn(ColorChannelEnum colorChannel)
        {
            return $"Select image for the {colorChannel} channel";
        }

        public const string CrtCombinedBtn = "Create Combined PNG";
        public const string CrtCombinedBtnNoInputs = "No Input Images";
        public const string CrtCombinedBtnGenerating = "Generating Combined PNG";

        public const string CrtExtractBtn = "Extract From Combined PNG";
        public const string CrtExtractBtnNoInputs = "No Input Image";
        public static string GetCrtExtractBtnDownload(ColorChannelEnum colorChannel)
        {
            return $"Download image of the {colorChannel} channel";
        }

        public const string SaveFileDialogTitle = "Save Preview As";
        public const string DownloadImgFileNameCombined = "CombinedImage";
        public static string GetDownloadImgFileNameExtractedChannel(ColorChannelEnum colorChannel)
        {
            return $"ExtractedChannel{colorChannel}";
        }

        // ---------------------------
        public static string GetExceptionError(Exception ex)
        {
            return $"Export Exception: {ex}";
        }
    }
}
