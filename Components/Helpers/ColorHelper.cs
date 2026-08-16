using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class ColorHelper
    {
        public static Color GetMediaColorHueAdjusted(Color color, float hueDegrees)
        {
            return ShiftHue(color, hueDegrees);
        }

        private static Color ShiftHue(Color color, float degrees)
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
    }
}
