using ImageCombinerChannelExtractor.Components.Helpers;
using System.IO;
using System.Text;
using System.Windows;
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

            StringBuilder combinedPattern = new StringBuilder();
            StringBuilder individualFilters = new StringBuilder();
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
        const float _brightenAmount = 0.1f;
        const float _darkenAmount = -0.1f;
        const float _desaturateAmount = 0.7f;
        private static readonly Color SharedColors_Red = Color.FromRgb(177, 0, 8);
        private static readonly Color SharedColors_Red_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Red, _brightenAmount);
        private static readonly Color SharedColors_Red_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Red, _desaturateAmount), _darkenAmount);

        private static readonly Color SharedColors_Green = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 120);
        private static readonly Color SharedColors_Green_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Green, _brightenAmount);
        private static readonly Color SharedColors_Green_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Green, _desaturateAmount), _darkenAmount * 2);

        private static readonly Color SharedColors_Blue = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 240);
        private static readonly Color SharedColors_Blue_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Blue, _brightenAmount * 2);
        private static readonly Color SharedColors_Blue_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Blue, _desaturateAmount), _darkenAmount / 16);

        private static readonly Color SharedColors_White = Color.FromRgb(68, 68, 68);
        private static readonly Color SharedColors_White_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_White, _brightenAmount / 2);
        private static readonly Color SharedColors_White_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_White, _desaturateAmount / 2), _darkenAmount);

        private static readonly Color SharedColors_Master = GetThemeColor("SolidBackgroundFillColorBaseBrush", Color.FromRgb(32, 32, 32));

        const byte _normalValue = 12;
        const byte _brightValue = (byte)(_normalValue * 1.2f);
        const byte _opaqueValue = 226;
        public static readonly Brush MainColorRed = CreateFrozenBrush(SharedColors_Red, _normalValue);
        public static readonly Brush MainColorRedBright = CreateFrozenBrush(SharedColors_Red_Bright, _brightValue);
        public static readonly Brush MainColorRedOpaque = CreateFrozenBrush(SharedColors_Red_Dark, _opaqueValue);
        public static readonly Brush MainColorGreen = CreateFrozenBrush(SharedColors_Green, _normalValue);
        public static readonly Brush MainColorGreenBright = CreateFrozenBrush(SharedColors_Green_Bright, _brightValue);
        public static readonly Brush MainColorGreenOpaque = CreateFrozenBrush(SharedColors_Green_Dark, _opaqueValue);
        public static readonly Brush MainColorBlue = CreateFrozenBrush(SharedColors_Blue, _normalValue);
        public static readonly Brush MainColorBlueBright = CreateFrozenBrush(SharedColors_Blue_Bright, _brightValue);
        public static readonly Brush MainColorBlueOpaque = CreateFrozenBrush(SharedColors_Blue_Dark, _opaqueValue);
        public static readonly Brush MainColorAlpha = CreateFrozenBrush(SharedColors_White, _normalValue);
        public static readonly Brush MainColorAlphaBright = CreateFrozenBrush(SharedColors_White_Bright, _brightValue);
        public static readonly Brush MainColorAlphaOpaque = CreateFrozenBrush(SharedColors_White_Dark, _opaqueValue);

        public static readonly Brush MainColorMasterOpaque = CreateFrozenBrush(SharedColors_Master, _opaqueValue);

        private static SolidColorBrush CreateFrozenBrush(Color color, byte alpha)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            brush.Freeze();
            return brush;
        }

        private static Color GetThemeColor(string resourceKey, Color fallbackColor)
        {
            if (Application.Current != null)
            {
                object resource = Application.Current.TryFindResource(resourceKey);
                if (resource is SolidColorBrush brush)
                {
                    return brush.Color;
                }
                if (resource is Color color)
                {
                    return color;
                }
            }
            return fallbackColor;
        }

        // notification delays
        public const int NotificationAutoDestroyAfterInSeconds = 300;
        public const int NotificationAutoDestroyAfterInSecondsIfException = NotificationAutoDestroyAfterInSeconds * 4;

        // extracting overlay
        public const int OverlayKeepOnScreenAfterFinishForInMilliseconds = 400;

        // preview sizes
        public const double CombinedPreviewDefaultSize = 425.0;
        public const double ExtractorPreviewDefaultSize = 425.0;
        public const double ExtractorPreviewOutputDefaultSize = 108.5;
    }
}
