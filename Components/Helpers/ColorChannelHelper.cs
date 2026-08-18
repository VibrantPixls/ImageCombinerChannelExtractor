using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Structs;
using ImageCombinerChannelExtractor.Components.UserControls;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class ColorChannelHelper
    {
        private static readonly Dictionary<ColorChannelEnum, ChannelSlot> _combinerChannels;
        private static readonly Dictionary<ColorChannelEnum, ChannelSlot> _extracterChannels;

        static ColorChannelHelper()
        {
            _combinerChannels = new Dictionary<ColorChannelEnum, ChannelSlot>
            {
                [ColorChannelEnum.Red] = new ChannelSlot(),
                [ColorChannelEnum.Green] = new ChannelSlot(),
                [ColorChannelEnum.Blue] = new ChannelSlot(),
                [ColorChannelEnum.Alpha] = new ChannelSlot(),
            };

            _extracterChannels = new Dictionary<ColorChannelEnum, ChannelSlot>
            {
                [ColorChannelEnum.Red] = new ChannelSlot(),
                [ColorChannelEnum.Green] = new ChannelSlot(),
                [ColorChannelEnum.Blue] = new ChannelSlot(),
                [ColorChannelEnum.Alpha] = new ChannelSlot(),
            };
        }

        #region Public getters
        public static Dictionary<ColorChannelEnum, ChannelSlot> GetCombinedChannelsDictionary()
        {
            return _combinerChannels;
        }

        public static CombinedImageTargetStruct GetCombinedImageTargetResolution(double combinedPreviewImageSizeDefault)
        {
            int maxWidth = 0;
            int maxHeight = 0;

            foreach (var entry in _combinerChannels)
            {
                var bitmap = entry.Value.Bitmap;
                if (bitmap is null)
                {
                    continue;
                }

                int width = bitmap.PixelWidth;
                int height = bitmap.PixelHeight;

                if (width > maxWidth)
                {
                    maxWidth = width;
                }

                if (height > maxHeight)
                {
                    maxHeight = height;
                }
            }
            return new CombinedImageTargetStruct((maxWidth, maxHeight), GetScaledDimensions(maxWidth, maxHeight, combinedPreviewImageSizeDefault));
        }
        #endregion

        // return true if an image was loaded
        public static bool LoadChannelFromPath(ColorChannelInput sender, ColorChannelEnum channel, string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
                sender.SetLabelText($"{Path.GetFileName(path)}");

                var newLoadedImage = LoadImageIndependent(path);
                targetDict[channel].Bitmap = newLoadedImage;
                targetDict[channel].BitmapSize = (newLoadedImage.PixelWidth, newLoadedImage.PixelHeight);
                targetDict[channel].FilePath = path;
                return true;
            }
            return false;
        }

        public static void DeleteChannel(ColorChannelInput sender, ColorChannelEnum channel)
        {
            var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            sender.SetLabelText(StringLinesInfo.NoInputImageTextDefault);
            targetDict[channel].Bitmap = null;
            targetDict[channel].BitmapSize = (0, 0);
            targetDict[channel].FilePath = string.Empty;
        }

        // filtering can only change for the combined channels
        public static void OnFilteringChanged(ColorChannelInput sender, RoutedEventArgs e)
        {
            _combinerChannels[sender.ColorChannel].FilteringMode = sender.SelectedFiltering;
        }

        #region Public helpers
        public static byte GetFilledColorChannelsAmount(bool IsCombined = true)
        {
            byte totalFilled = 0;
            foreach (var channel in IsCombined ? _combinerChannels.Values : _extracterChannels.Values)
            {
                var bitmap = channel.Bitmap;
                if (bitmap != null)
                {
                    totalFilled++;
                }
            }
            return totalFilled;
        }

        public static bool AreChannelsMismatchedInSize(bool IsCombined = true)
        {
            int firstWidth = -1;
            int firstHeight = -1;

            foreach (var channel in IsCombined ? _combinerChannels.Values : _extracterChannels.Values)
            {
                var bitmap = channel.Bitmap;
                if (bitmap == null)
                {
                    continue;
                }

                if (firstWidth == -1)
                {
                    firstWidth = bitmap.PixelWidth;
                    firstHeight = bitmap.PixelHeight;
                }
                else if (bitmap.PixelWidth != firstWidth || bitmap.PixelHeight != firstHeight)
                {
                    return true;
                }
            }
            return false;
        }

        public static (int Width, int Height) GetColorChannelImageSize(ColorChannelInput sender)
        {
            var dict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            var bitmapSize = dict[sender.ColorChannel].BitmapSize;
            return (bitmapSize.Width, bitmapSize.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesSenderInputChannelHaveInputImage(ColorChannelInput sender)
        {
            var dict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            return dict[sender.ColorChannel].Bitmap != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesSenderInputChannelHaveDifferentImage(ColorChannelInput sender, string? newPath)
        {
            if (string.IsNullOrEmpty(newPath))
            {
                return false;
            }

            var dict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            if (!DoesSenderInputChannelHaveInputImage(dict, sender))
            {
                return true; // otherwise first inputs won't load
            }
            return !string.Equals(dict[sender.ColorChannel].FilePath, newPath, StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesAnyChannelHaveInputImages()
        {
            bool someChannelHasAInputImage = false;
            foreach (var source in _combinerChannels)
            {
                if (source.Value.Bitmap != null)
                {
                    someChannelHasAInputImage = true;
                    break;
                }
            }
            return someChannelHasAInputImage;
        }
        #endregion

        #region Private channel
        private static BitmapImage LoadImageIndependent(string path)
        {
            // Make sure the input file doesn't lock
            var bitmap = new BitmapImage();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }
        #endregion

        #region Private helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (double Width, double Height) GetScaledDimensions(double width, double height, double combinedPreviewImageSizeDefault)
        {
            double scale = GetScaleFactor(width, height, combinedPreviewImageSizeDefault);
            return (width * scale, height * scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetScaleFactor(double width, double height, double combinedPreviewImageSizeDefault)
        {
            double maxDimension = Math.Max(width, height);
            return combinedPreviewImageSizeDefault / maxDimension;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoesSenderInputChannelHaveInputImage(Dictionary<ColorChannelEnum, ChannelSlot> dictionary, ColorChannelInput sender)
        {
            return dictionary[sender.ColorChannel].Bitmap != null;
        }

        #endregion
    }
}