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

        // colors
        const float _brightenAmount = 0.1f;
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
    }
}
