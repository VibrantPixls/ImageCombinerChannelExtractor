using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class ColorHelper
    {
        private static readonly Brush[] _brushes =
        [
            SharedInfo.MainColorRed, SharedInfo.MainColorRedBright, SharedInfo.MainColorRedOpaque,
            SharedInfo.MainColorGreen, SharedInfo.MainColorGreenBright, SharedInfo.MainColorGreenOpaque,
            SharedInfo.MainColorBlue, SharedInfo.MainColorBlueBright, SharedInfo.MainColorBlueOpaque,
            SharedInfo.MainColorAlpha, SharedInfo.MainColorAlphaBright, SharedInfo.MainColorAlphaOpaque
        ];

        public static Brush GetColorBrush(ColorChannelEnum channel, ColorBrushTypeEnum brushType = ColorBrushTypeEnum.Normal)
        {
            return _brushes[(byte)channel * 3 + (byte)brushType];
        }
        public static Brush GetColorBrushForMaster()
        {
            return SharedInfo.MainColorMasterOpaque;
        }

        public static Color GetMediaColorHueAdjusted(Color color, float degrees)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = MathF.Max(r, MathF.Max(g, b));
            float min = MathF.Min(r, MathF.Min(g, b));
            float delta = max - min;

            float h;
            if (delta == 0f)
            {
                h = 0f;
            }
            else if (max == r)
            {
                h = 60f * (((g - b) / delta) % 6f);
            }
            else if (max == g)
            {
                h = 60f * (((b - r) / delta) + 2f);
            }
            else
            {
                h = 60f * (((r - g) / delta) + 4f);
            }

            if (h < 0f)
            {
                h += 360f;
            }

            float s = max == 0f ? 0f : delta / max;
            float v = max;

            h += degrees;
            h %= 360f;
            if (h < 0f)
            {
                h += 360f;
            }

            if (delta == 0f)
            {
                return color;
            }

            float c = v * s;
            float x = c * (1f - MathF.Abs((h / 60f) % 2f - 1f));
            float m = v - c;

            float r1, g1, b1;
            if (h < 60f)
            {
                r1 = c; g1 = x; b1 = 0f;
            }
            else if (h < 120f)
            {
                r1 = x; g1 = c; b1 = 0f;
            }
            else if (h < 180f)
            {
                r1 = 0f; g1 = c; b1 = x;
            }
            else if (h < 240f)
            {
                r1 = 0f; g1 = x; b1 = c;
            }
            else if (h < 300f)
            {
                r1 = x; g1 = 0f; b1 = c;
            }
            else
            {
                r1 = c; g1 = 0f; b1 = x;
            }

            byte rb = (byte)MathF.Round((r1 + m) * 255f);
            byte gb = (byte)MathF.Round((g1 + m) * 255f);
            byte bb = (byte)MathF.Round((b1 + m) * 255f);

            return Color.FromArgb(color.A, rb, gb, bb);
        }

        public static Color GetMediaColorBrighter(Color color, float amount)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float max = MathF.Max(r, MathF.Max(g, b));
            float min = MathF.Min(r, MathF.Min(g, b));
            float d = max - min;
            float v = MathF.Max(0f, MathF.Min(1f, max + amount));

            if (d == 0f)
            {
                byte gray = (byte)MathF.Round(v * 255f);
                return Color.FromArgb(color.A, gray, gray, gray);
            }

            float s = max == 0f ? 0f : d / max;
            float scale = max == 0f ? 0f : v / max;

            byte rb = (byte)MathF.Round(MathF.Min(255f, r * scale * 255f));
            byte gb = (byte)MathF.Round(MathF.Min(255f, g * scale * 255f));
            byte bb = (byte)MathF.Round(MathF.Min(255f, b * scale * 255f));

            return Color.FromArgb(color.A, rb, gb, bb);
        }

        public static Color GetMediaColorDesaturated(Color color, float amount)
        {
            amount = MathF.Max(0f, MathF.Min(1f, amount));
            float grayscale = 0.299f * color.R + 0.587f * color.G + 0.114f * color.B;
            byte rb = (byte)MathF.Round(color.R + (grayscale - color.R) * amount);
            byte gb = (byte)MathF.Round(color.G + (grayscale - color.G) * amount);
            byte bb = (byte)MathF.Round(color.B + (grayscale - color.B) * amount);
            return Color.FromArgb(color.A, rb, gb, bb);
        }
    }
}
