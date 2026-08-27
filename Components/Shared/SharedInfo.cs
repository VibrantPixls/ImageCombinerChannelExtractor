using ImageCombinerChannelExtractor.Components.Helpers;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

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
        private static Color SharedColors_Red;
        private static Color SharedColors_Red_Bright;
        private static Color SharedColors_Red_Dark;

        private static Color SharedColors_Green;
        private static Color SharedColors_Green_Bright;
        private static Color SharedColors_Green_Dark;

        private static Color SharedColors_Blue;
        private static Color SharedColors_Blue_Bright;
        private static Color SharedColors_Blue_Dark;

        private static Color SharedColors_White;
        private static Color SharedColors_White_Bright;
        private static Color SharedColors_White_Dark;

        private static Color SharedColors_Master;

        public static SolidColorBrush MainColorRed = new SolidColorBrush();
        public static SolidColorBrush MainColorRedBright = new SolidColorBrush();
        public static SolidColorBrush MainColorRedOpaque = new SolidColorBrush();
        public static SolidColorBrush MainColorGreen = new SolidColorBrush();
        public static SolidColorBrush MainColorGreenBright = new SolidColorBrush();
        public static SolidColorBrush MainColorGreenOpaque = new SolidColorBrush();
        public static SolidColorBrush MainColorBlue = new SolidColorBrush();
        public static SolidColorBrush MainColorBlueBright = new SolidColorBrush();
        public static SolidColorBrush MainColorBlueOpaque = new SolidColorBrush();
        public static SolidColorBrush MainColorAlpha = new SolidColorBrush();
        public static SolidColorBrush MainColorAlphaBright = new SolidColorBrush();
        public static SolidColorBrush MainColorAlphaOpaque = new SolidColorBrush();

        public static SolidColorBrush MainColorMasterOpaque = new SolidColorBrush();

        private static SolidColorBrush UpdateBrush(this SolidColorBrush brush, Color color, byte alpha)
        {
            Color targetColor = color with { A = alpha };
            brush.Color = targetColor;
            return brush;
        }

        private static void UpdateMainColors(float multiplier = 1f)
        {
            float _brightenAmount = 0.1f * multiplier;
            float _darkenAmount = -0.1f * multiplier;
            const float _desaturateAmount = 0.7f;

            SharedColors_Red = Color.FromRgb(177, 0, 8);
            SharedColors_Red_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Red, _brightenAmount);
            SharedColors_Red_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Red, _desaturateAmount), _darkenAmount);

            SharedColors_Green = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 120);
            SharedColors_Green_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Green, _brightenAmount);
            SharedColors_Green_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Green, _desaturateAmount), _darkenAmount * 2);

            SharedColors_Blue = ColorHelper.GetMediaColorHueAdjusted(SharedColors_Red, 240);
            SharedColors_Blue_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_Blue, _brightenAmount * 2);
            SharedColors_Blue_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_Blue, _desaturateAmount), _darkenAmount / 16);

            SharedColors_White = Color.FromRgb(68, 68, 68);
            SharedColors_White_Bright = ColorHelper.GetMediaColorBrighter(SharedColors_White, _brightenAmount / 2);
            SharedColors_White_Dark = ColorHelper.GetMediaColorBrighter(ColorHelper.GetMediaColorDesaturated(SharedColors_White, _desaturateAmount / 2), _darkenAmount);

            SharedColors_Master = GetThemeColorColor("SolidBackgroundFillColorBaseBrush", Color.FromRgb(32, 32, 32));

            const byte _normalAlphaValue = 12;
            const byte _brightAlphaValue = (byte)(_normalAlphaValue * 1.2f);
            const byte _opaqueAlphaValue = 226;
            MainColorRed.UpdateBrush(SharedColors_Red, _normalAlphaValue);
            MainColorRedBright.UpdateBrush(SharedColors_Red_Bright, _brightAlphaValue);
            MainColorRedOpaque.UpdateBrush(SharedColors_Red_Dark, _opaqueAlphaValue);
            MainColorGreen.UpdateBrush(SharedColors_Green, _normalAlphaValue);
            MainColorGreenBright.UpdateBrush(SharedColors_Green_Bright, _brightAlphaValue);
            MainColorGreenOpaque.UpdateBrush(SharedColors_Green_Dark, _opaqueAlphaValue);
            MainColorBlue.UpdateBrush(SharedColors_Blue, _normalAlphaValue);
            MainColorBlueBright.UpdateBrush(SharedColors_Blue_Bright, _brightAlphaValue);
            MainColorBlueOpaque.UpdateBrush(SharedColors_Blue_Dark, _opaqueAlphaValue);
            MainColorAlpha.UpdateBrush(SharedColors_White, _normalAlphaValue);
            MainColorAlphaBright.UpdateBrush(SharedColors_White_Bright, _brightAlphaValue);
            MainColorAlphaOpaque.UpdateBrush(SharedColors_White_Dark, _opaqueAlphaValue);

            MainColorMasterOpaque.UpdateBrush(SharedColors_Master, _opaqueAlphaValue);
        }

        public static void UpdateColorBrushes(ApplicationTheme theme)
        {
            UpdateMainColors(theme == ApplicationTheme.Light ? -1f : 1);
        }

        public static readonly Color ApplicationAccentColor = Color.FromRgb(0, 120, 212);

        private static Color GetThemeColorColor(string resourceKey, Color fallbackColor)
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
        public const int NotificationAutoDestroyAfterInSeconds = 3;

        // extracting overlay
        public const int OverlayKeepOnScreenAfterFinishForInMilliseconds = 400;

        // preview sizes
        public const double CombinedPreviewDefaultSize = 425.0;
        public const double ExtractorPreviewDefaultSize = 425.0;
        public const double ExtractorPreviewOutputDefaultSize = 108.5;
    }
}
