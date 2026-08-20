using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Pages;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Structs;
using ImageCombinerChannelExtractor.Components.UserControls;
using Microsoft.Win32;
using System.IO;
using System.Runtime.CompilerServices;
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

        #region Extractor
        #region Public getters
        public static Dictionary<ColorChannelEnum, ChannelSlot> GetExtractedChannelsDictionary()
        {
            return _extracterChannels;
        }

        public static ChannelSlot GetExtractedInput()
        {
            return _extractorInput;
        }
        #endregion

        public static (double Width, double Height) GetTargetResolution()
        {
            var size = _extractorInput.BitmapSize;
            return GetScaledDimensions(size.Width, size.Height, SharedInfo.ExtractorPreviewDefaultSize);
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

        public static void DeleteChannel(ExtractorPage sender)
        {
            sender.UpdateLabel(StringLinesInfo.NoInputImageTextDefault);
            ClearBitmap(_extractorInput);
        }
        #endregion

        #region Combiner
        #region Public getters
        public static Dictionary<ColorChannelEnum, ChannelSlot> GetCombinedChannelsDictionary()
        {
            return _combinerChannels;
        }

        public static CombinedImageTargetStruct GetCombinedImageTargetResolution()
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
            return new CombinedImageTargetStruct((maxWidth, maxHeight), GetScaledDimensions(maxWidth, maxHeight, SharedInfo.CombinedPreviewDefaultSize));
        }
        #endregion

        // return true if an image was loaded
        public static bool LoadChannelFromPath(ColorChannelUserControl sender, string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
                sender.SetLabelText($"{Path.GetFileName(path)}");
                AddBitmap(targetDict[sender.ColorChannel], LoadImageIndependent(path), path);
                return true;
            }
            return false;
        }

        public static void DeleteChannel(ColorChannelUserControl sender)
        {
            var targetDict = sender.IsChannelFromCombined ? _combinerChannels : _extracterChannels;
            sender.SetLabelText(StringLinesInfo.NoInputImageTextDefault);
            ClearBitmap(targetDict[sender.ColorChannel]);
        }

        // filtering can only change for the combined channels
        public static void OnFilteringChanged(ColorChannelInput sender)
        {
            _combinerChannels[sender.ColorChannel].FilteringMode = sender.SelectedFiltering;
        }

        #region Public helpers
        public static byte GetFilledCombinedColorChannelsAmount()
        {
            byte totalFilled = 0;
            foreach (var channel in _combinerChannels)
            {
                var bitmap = channel.Value.Bitmap;
                if (bitmap != null)
                {
                    totalFilled++;
                }
            }
            return totalFilled;
        }

        public static bool AreCombinedChannelsMismatchedInSize()
        {
            int firstWidth = -1;
            int firstHeight = -1;

            foreach (var channel in _combinerChannels)
            {
                var bitmap = channel.Value.Bitmap;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesSenderInputChannelHaveDifferentImageForCombined(ColorChannelEnum channel, string? newPath)
        {
            if (string.IsNullOrEmpty(newPath))
            {
                return false;
            }

            if (!DoesSenderInputChannelHaveInputImageForCombined(channel))
            {
                return true; // otherwise first inputs won't load
            }
            return !string.Equals(_combinerChannels[channel].FilePath, newPath, StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesAnyChannelHaveInputImagesForCombined()
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

        public static BitmapScalingMode GetBitmapScalingFilteringMode(ColorChannelEnum channel)
        {
            return _combinerChannels[channel].FilteringMode switch
            {
                ChannelFilteringMode.Bilinear => BitmapScalingMode.Linear,
                ChannelFilteringMode.NearestNeighbor => BitmapScalingMode.NearestNeighbor,
                _ => BitmapScalingMode.HighQuality
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DoesSenderInputChannelHaveInputImageForCombined(ColorChannelEnum channel)
        {
            return _combinerChannels[channel].Bitmap != null;
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
        #endregion
        #endregion

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
        private static void AddBitmap(ChannelSlot slotToAdd, BitmapImage image, string path)
        {
            slotToAdd.Bitmap = image;
            slotToAdd.BitmapSize = (image.PixelWidth, image.PixelHeight);
            slotToAdd.FilePath = path;
        }

        private static void ClearBitmap(ChannelSlot slotToClear)
        {
            slotToClear.Bitmap = null;
            slotToClear.BitmapSize = (0, 0);
            slotToClear.FilePath = string.Empty;
        }
        #endregion
    }
}