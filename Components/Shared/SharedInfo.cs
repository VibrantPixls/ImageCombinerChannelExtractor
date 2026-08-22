using ImageCombinerChannelExtractor.Components.Helpers;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Shared
{
    public static class SharedInfo
    {
        // image files
        public static IReadOnlyList<string> AllowedImageExtensions { get; } = [".png", ".jpg", ".jpeg"];
        public static string OpenFileDialogFilter { get; } = BuildFileDialogFilter(AllowedImageExtensions);
        public static IReadOnlyList<string> AllowedImageExtensionsSaving { get; } = [".png"];
        public static string SaveFileDialogFilter { get; } = BuildFileDialogFilter(AllowedImageExtensionsSaving);

        private static string BuildFileDialogFilter(IReadOnlyList<string> extensions)
        {
            if (extensions == null || extensions.Count == 0)
            {
                return string.Empty;
            }

            var combinedPattern = new StringBuilder();
            var individualFilters = new StringBuilder();
            for (int i = 0; i < extensions.Count; i++)
            {
                string ext = extensions[i].TrimStart('.').ToLowerInvariant();
                string extUpper = ext.ToUpperInvariant();
                if (i > 0)
                {
                    combinedPattern.Append(';');
                    individualFilters.Append('|');
                }
                combinedPattern.Append("*.").Append(ext);
                individualFilters.Append(extUpper).Append(" Files (*.").Append(ext).Append(")|*.").Append(ext);
            }
            return $"All Supported Images ({combinedPattern})|{combinedPattern}|{individualFilters}";
        }

        // image files
        public static bool IsValidImageFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string extension = Path.GetExtension(filePath);
            return AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        // colors
        //const float _brightenAmount = 0.1f;
        const float _brightenAmount = 1.1f;
        private static readonly Color SharedColors_Red = Color.FromRgb(177, 0, 8);
        private static readonly Color SharedColors_Red_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Red, _brightenAmount);

        private static readonly Color SharedColors_Green = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 120);
        private static readonly Color SharedColors_Green_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Green, _brightenAmount);

        private static readonly Color SharedColors_Blue = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 240);
        private static readonly Color SharedColors_Blue_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Blue, _brightenAmount);

        private static readonly Color SharedColors_White = Color.FromRgb(68, 68, 68);
        private static readonly Color SharedColors_White_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_White, _brightenAmount / 2);

        public static readonly Brush MainColorRed = new SolidColorBrush(SharedColors_Red);
        public static readonly Brush MainColorRedBright = new SolidColorBrush(SharedColors_Red_Bright);
        public static readonly Brush MainColorGreen = new SolidColorBrush(SharedColors_Green);
        public static readonly Brush MainColorGreenBright = new SolidColorBrush(SharedColors_Green_Bright);
        public static readonly Brush MainColorBlue = new SolidColorBrush(SharedColors_Blue);
        public static readonly Brush MainColorBlueBright = new SolidColorBrush(SharedColors_Blue_Bright);
        public static readonly Brush MainColorAlpha = new SolidColorBrush(SharedColors_White);
        public static readonly Brush MainColorAlphaBright = new SolidColorBrush(SharedColors_White_Bright);

        // notification delays
        public const int NotificationAutoDestroyAfterInSeconds = 3;
        public const int NotificationAutoDestroyAfterInSecondsIfException = NotificationAutoDestroyAfterInSeconds * 4;

        // extracting overlay
        public const int OverlayKeepOnScreenAfterFinishForInMilliseconds = 400;

        // preview sizes
        public const double CombinedPreviewDefaultSize = 425.0;
        public const double ExtractorPreviewDefaultSize = 425.0;
    }
}
