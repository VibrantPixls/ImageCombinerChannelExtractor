using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Pages;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Structs;
using ImageCombinerChannelExtractor.Components.UserControls;
using Microsoft.Win32;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class ColorChannelHelper
    {
        private static readonly Dictionary<ColorChannelEnum, ChannelSlot> _combinerChannels;
        private static readonly Dictionary<ColorChannelEnum, ChannelSlot> _extracterChannels;
        private static readonly ChannelSlot _extractorInput;

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
            _extractorInput = new ChannelSlot();
        }

        #region Public getters
        public static Dictionary<ColorChannelEnum, ChannelSlot> GetCombinedChannelsDictionary()
        {
            return _combinerChannels;
        }

        public static Dictionary<ColorChannelEnum, ChannelSlot> GetExtractedChannelsDictionary()
        {
            return _extracterChannels;
        }

        public static ChannelSlot GetExtractedInput()
        {
            return _extractorInput;
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
        public static bool LoadChannelFromPath(ColorChannelUserControl sender, ColorChannelEnum channel, string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
                sender.SetLabelText($"{Path.GetFileName(path)}");
                AddBitmap(targetDict[channel], LoadImageIndependent(path), path);
                return true;
            }
            return false;
        }

        public static bool LoadChannelFromPath(ExtractorPage sender, string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                sender.UpdateLabel($"{Path.GetFileName(path)}");
                AddBitmap(_extractorInput, LoadImageIndependent(path), path);
                return true;
            }
            return false;
        }

        public static void DeleteChannel(ColorChannelUserControl sender, ColorChannelEnum channel)
        {
            var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            sender.SetLabelText(StringLinesInfo.NoInputImageTextDefault);
            ClearBitmap(targetDict[channel]);
        }

        public static void DeleteChannel(ExtractorPage sender)
        {
            sender.UpdateLabel(StringLinesInfo.NoInputImageTextDefault);
            ClearBitmap(_extractorInput);
        }

        // filtering can only change for the combined channels
        public static void OnFilteringChanged(ColorChannelInput sender, RoutedEventArgs e)
        {
            _combinerChannels[sender.ColorChannel].FilteringMode = sender.SelectedFiltering;
        }

        #region Public helpers
        public static string? SelectPNGFile()
        {
            OpenFileDialog ofd = new()
            {
                Filter = SharedInfo.OpenFileDialogFilter
            };
            return ofd.ShowDialog() == true ? ofd.FileName : null;
        }

        public static bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string extension = Path.GetExtension(filePath);
            return SharedInfo.AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

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

        public static BitmapScalingMode GetBitmapScalingFilteringMode(ColorChannelInput sender)
        {
            var dict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            return dict[sender.ColorChannel].FilteringMode switch
            {
                ChannelFilteringMode.Bilinear => BitmapScalingMode.Linear,
                ChannelFilteringMode.NearestNeighbor => BitmapScalingMode.NearestNeighbor,
                _ => BitmapScalingMode.HighQuality
            };
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
        private static void AddBitmap(ChannelSlot slotToClear, BitmapImage image, string path)
        {
            slotToClear.Bitmap = image;
            slotToClear.BitmapSize = (image.PixelWidth, image.PixelHeight);
            slotToClear.FilePath = path;
        }

        private static void ClearBitmap(ChannelSlot slotToClear)
        {
            slotToClear.Bitmap = null;
            slotToClear.BitmapSize = (0, 0);
            slotToClear.FilePath = string.Empty;
        }

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