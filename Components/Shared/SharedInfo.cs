using ImageCombinerChannelExtractor.Components.Helpers;
using System.IO;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Shared
{
    public static class SharedInfo
    {
        public static readonly string[] AllowedImageExtensions = { ".png", ".jpg", ".jpeg" };
        public static readonly string OpenFileDialogFilter;

        static SharedInfo()
        {
            var extsWithoutDot = AllowedImageExtensions.Select(ext => ext.TrimStart('.').ToLower()).ToArray();
            string combinedPattern = string.Join(";", extsWithoutDot.Select(ext => $"*.{ext}"));
            string individualFilters = string.Join("|", extsWithoutDot.Select(ext => $"{ext.ToUpper()} Files (*.{ext})|*.{ext}"));
            OpenFileDialogFilter = $"All Supported Images ({combinedPattern})|{combinedPattern}|{individualFilters}";
        }

        public static bool IsValidImageFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string extension = Path.GetExtension(filePath);
            return AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static readonly Color baseRedColor = Color.FromRgb(177, 0, 8);
        private static readonly Color baseRedWhite = Color.FromRgb(68, 64, 59);

        public static readonly Brush MainColorRed = new SolidColorBrush(baseRedColor);
        public static readonly Brush MainColorGreen = new SolidColorBrush(ColorHelper.GetMediaColorHueAdjusted(baseRedColor, 120));
        public static readonly Brush MainColorBlue = new SolidColorBrush(ColorHelper.GetMediaColorHueAdjusted(baseRedColor, 240));
        public static readonly Brush MainColorAlpha = new SolidColorBrush(baseRedWhite);
    }
}
