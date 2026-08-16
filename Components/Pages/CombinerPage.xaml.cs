using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.UserControls;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class CombinerPage : Page
    {
        #region Image previews variables
        private const double _combinedDefaultSize = 425.0;

        private CurrentBitmapPreviewingEnum _currentlyPreviewingType = CurrentBitmapPreviewingEnum.None;
        private ColorChannelEnum? _currentlyPreviewingColorChannel = null;
        #endregion

        #region Combined ouput image variables
        private static bool _differentResolutionsForCombinedOutput;
        private static int _resolutionCombinedOutputWidth = -1;
        private static int _resolutionCombinedOutputHeight = -1;

        private BitmapSource? _combinedPreviewCache = null;
        private bool _isWaitingForCombinedImage = false;
        private CancellationTokenSource? _ctsCombined;
        #endregion

        public CombinerPage()
        {
            InitializeComponent();

            brdPreviewBoxCombined.Width = _combinedDefaultSize;
            brdPreviewBoxCombined.Height = _combinedDefaultSize;
        }

        #region On ColorChannelInput events
        private void OnChannelClick(object sender, FileSelectedEventArgs e)
        {
            var input = (ColorChannelInput)sender;
            string? path = e.SelectedFilePath ?? SelectPNGFile();

            LoadChannelFromPath(input, input.ColorChannel, path);
        }

        private void OnChannelClickRemove(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelInput)sender;
            DeleteChannel(input, input.ColorChannel);
        }

        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelInput)sender;
            ShowChannelPreview(input, input.ColorChannel);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelInput)sender;
        }

        private void OnButtonMouseEnterCombined(object sender, MouseEventArgs e)
        {
            ShowCombinedPreview();
        }

        private void OnFilteringChanged(object sender, RoutedEventArgs e)
        {
            ColorChannelHelper.OnFilteringChanged(sender, e);
            MarkCombinedPreviewDirty();
        }
        #endregion

        #region Load images into channels
        private void LoadChannelFromPath(ColorChannelInput sender, ColorChannelEnum channel, string? path)
        {
            if (ColorChannelHelper.LoadChannelFromPath(sender, channel, path))
            {
                ShowChannelPreview(sender, channel, true);
            }
            UpdateResolution();

            MarkCombinedPreviewDirty();
        }

        private void DeleteChannel(ColorChannelInput sender, ColorChannelEnum channel)
        {
            ColorChannelHelper.DeleteChannel(sender, channel);

            ClearChannelPreview();
            UpdateResolution();

            MarkCombinedPreviewDirty();
        }

        private static string? SelectPNGFile()
        {
            OpenFileDialog ofd = new()
            {
                Filter = SharedInfo.OpenFileDialogFilter
            };
            return ofd.ShowDialog() == true ? ofd.FileName : null;
        }

        private void UpdateResolution()
        {
            byte filledChannels = ColorChannelHelper.GetFilledColorChannelsAmount();
            if (filledChannels == 0)
            {
                _differentResolutionsForCombinedOutput = false;
                _resolutionCombinedOutputWidth = 0;
                _resolutionCombinedOutputHeight = 0;
                lblResolutionCombined.Text = string.Empty;

                brdPreviewBoxCombined.Width = _combinedDefaultSize;
                brdPreviewBoxCombined.Height = _combinedDefaultSize;
                return;
            }

            _differentResolutionsForCombinedOutput = ColorChannelHelper.AreChannelsMismatchedInSize();
            UpdateTargetResolutionCombined();

            lblResolutionCombined.Text = _differentResolutionsForCombinedOutput ? $"Mismatched sizes - output will be {_resolutionCombinedOutputWidth}x{_resolutionCombinedOutputHeight}" : $"{_resolutionCombinedOutputWidth}x{_resolutionCombinedOutputHeight}";
        }
        #endregion

        #region Combiner
        public void MarkCombinedPreviewDirty()
        {
            _combinedPreviewCache = null;
            _ = GenerateCombinedPreviewAsync();
        }

        private async Task GenerateCombinedPreviewAsync()
        {
            if (!ColorChannelHelper.DoesAnyChannelHaveInputImages())
            {
                //early exit when no input images
                return;
            }

            _ctsCombined?.Cancel();
            _ctsCombined?.Dispose();
            var cts = new CancellationTokenSource();
            _ctsCombined = cts;
            var token = cts.Token;

            var notifInt = TriggerNotification(TextInfo.notificationCombining, NotificationTypeEnum.Combining);
            try
            {
                await Task.Delay(10, token).ConfigureAwait(true);

                // Snapshot it before doing anything
                var snapshot = new Dictionary<ColorChannelEnum, ChannelSlot>(ColorChannelHelper.GetCombinedChannelsDictionary());

                BitmapSource? result = await Task.Run(() => BuildCombinedImage(snapshot, token), token).ConfigureAwait(true);

                if (!token.IsCancellationRequested)
                {
                    _combinedPreviewCache = result;
                    // now show if the user was waiting for it
                    StartShowingCombinedPreview();
                    TriggerNotification(TextInfo.notificationSuccessfullCombining, NotificationTypeEnum.Success, true);
                }
            }
            catch (OperationCanceledException)
            {
                // cancelled

            }
            finally
            {
                TriggerRemoveNotification(notifInt);
            }
        }

        private static BitmapSource? BuildCombinedImage(Dictionary<ColorChannelEnum, ChannelSlot> channels, CancellationToken token)
        {
            var sources = channels.Where(kv => kv.Value?.Bitmap != null).ToDictionary(kv => kv.Key, kv => kv.Value!);
            if (sources.Count == 0)
            {
                return null;
            }

            token.ThrowIfCancellationRequested();

            byte[]? r = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.Red), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? g = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.Green), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? b = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.Blue), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? a = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.Alpha), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);

            token.ThrowIfCancellationRequested();

            int stride = _resolutionCombinedOutputWidth * 4;
            byte[] pixels = new byte[stride * _resolutionCombinedOutputHeight];

            Parallel.For(0, _resolutionCombinedOutputHeight, new ParallelOptions { CancellationToken = token }, y =>
            {
                int rowOffset = y * _resolutionCombinedOutputWidth;
                int pixelRowOffset = y * stride;

                for (int x = 0; x < _resolutionCombinedOutputWidth; x++)
                {
                    int srcIdx = rowOffset + x;
                    int dstIdx = pixelRowOffset + x * 4;

                    pixels[dstIdx + 0] = b?[srcIdx] ?? 0;
                    pixels[dstIdx + 1] = g?[srcIdx] ?? 0;
                    pixels[dstIdx + 2] = r?[srcIdx] ?? 0;
                    pixels[dstIdx + 3] = a?[srcIdx] ?? 255;
                }
            });
            token.ThrowIfCancellationRequested();

            BitmapSource combined = BitmapSource.Create(_resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
            combined.Freeze();
            return combined;
        }

        private static byte[]? GetChannelGrayscaleBuffer(ChannelSlot? channelSource, int targetWidth, int targetHeight, CancellationToken token)
        {
            if (channelSource?.Bitmap is not { } image)
            {
                return null;
            }

            var source = (BitmapSource)image;
            int nativeW = source.PixelWidth;
            int nativeH = source.PixelHeight;

            var grayscaleConverter = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            var nativeBuffer = new byte[nativeW * nativeH];
            grayscaleConverter.CopyPixels(nativeBuffer, nativeW, 0);

            token.ThrowIfCancellationRequested();

            double srcAspect = (double)nativeW / nativeH;
            double targetAspect = (double)targetWidth / targetHeight;

            int drawWidth, drawHeight;
            if (srcAspect > targetAspect)
            {
                drawWidth = targetWidth;
                drawHeight = Math.Max(1, (int)Math.Round(targetWidth / srcAspect));
            }
            else
            {
                drawHeight = targetHeight;
                drawWidth = Math.Max(1, (int)Math.Round(targetHeight * srcAspect));
            }

            byte[] resized = channelSource.FilteringMode switch
            {
                ChannelFilteringMode.NearestNeighbor => ResizeNearestNeighbor(nativeBuffer, nativeW, nativeH, drawWidth, drawHeight),
                ChannelFilteringMode.Bilinear => ResizeBilinear(nativeBuffer, nativeW, nativeH, drawWidth, drawHeight),
                ChannelFilteringMode.Bicubic => ResizeBicubic(nativeBuffer, nativeW, nativeH, drawWidth, drawHeight),
                _ => ResizeBilinear(nativeBuffer, nativeW, nativeH, drawWidth, drawHeight)
            };

            if (drawWidth == targetWidth && drawHeight == targetHeight)
            {
                return resized;
            }

            var canvas = new byte[targetWidth * targetHeight];
            int offsetX = (targetWidth - drawWidth) / 2;
            int offsetY = (targetHeight - drawHeight) / 2;

            for (int y = 0; y < drawHeight; y++)
            {
                token.ThrowIfCancellationRequested();
                Buffer.BlockCopy(resized, y * drawWidth, canvas, (y + offsetY) * targetWidth + offsetX, drawWidth);
            }
            return canvas;
        }

        #region Filtering helpers
        private static byte[] ResizeNearestNeighbor(byte[] src, int srcW, int srcH, int dstW, int dstH)
        {
            if (srcW == dstW && srcH == dstH)
            {
                return src;
            }

            var dst = new byte[dstW * dstH];
            double xRatio = (double)srcW / dstW;
            double yRatio = (double)srcH / dstH;

            Parallel.For(0, dstH, y =>
            {
                int srcY = Math.Min(srcH - 1, (int)(y * yRatio));
                int dstRow = y * dstW;
                int srcRow = srcY * srcW;

                for (int x = 0; x < dstW; x++)
                {
                    int srcX = Math.Min(srcW - 1, (int)(x * xRatio));
                    dst[dstRow + x] = src[srcRow + srcX];
                }
            });
            return dst;
        }

        private static byte[] ResizeBilinear(byte[] src, int srcW, int srcH, int dstW, int dstH)
        {
            if (srcW == dstW && srcH == dstH)
            {
                return src;
            }

            var dst = new byte[dstW * dstH];
            double xRatio = dstW > 1 ? (double)(srcW - 1) / (dstW - 1) : 0;
            double yRatio = dstH > 1 ? (double)(srcH - 1) / (dstH - 1) : 0;

            Parallel.For(0, dstH, y =>
            {
                double srcYf = y * yRatio;
                int y0 = (int)srcYf;
                int y1 = Math.Min(srcH - 1, y0 + 1);
                double fy = srcYf - y0;

                int dstRow = y * dstW;

                for (int x = 0; x < dstW; x++)
                {
                    double srcXf = x * xRatio;
                    int x0 = (int)srcXf;
                    int x1 = Math.Min(srcW - 1, x0 + 1);
                    double fx = srcXf - x0;

                    double top = src[y0 * srcW + x0] * (1 - fx) + src[y0 * srcW + x1] * fx;
                    double bottom = src[y1 * srcW + x0] * (1 - fx) + src[y1 * srcW + x1] * fx;
                    double value = top * (1 - fy) + bottom * fy;

                    dst[dstRow + x] = (byte)Math.Round(value);
                }
            });
            return dst;
        }

        private static byte[] ResizeBicubic(byte[] src, int srcW, int srcH, int dstW, int dstH)
        {
            if (srcW == dstW && srcH == dstH)
            {
                return src;
            }

            var dst = new byte[dstW * dstH];
            double xRatio = (double)srcW / dstW;
            double yRatio = (double)srcH / dstH;

            Parallel.For(0, dstH, y =>
            {
                double srcYf = (y + 0.5) * yRatio - 0.5;
                int y1 = (int)Math.Floor(srcYf);
                double fy = srcYf - y1;

                int dstRow = y * dstW;

                for (int x = 0; x < dstW; x++)
                {
                    double srcXf = (x + 0.5) * xRatio - 0.5;
                    int x1 = (int)Math.Floor(srcXf);
                    double fx = srcXf - x1;

                    double result = 0;
                    for (int m = -1; m <= 2; m++)
                    {
                        int sy = Math.Clamp(y1 + m, 0, srcH - 1);
                        double wy = CubicWeight(m - fy);

                        double rowSum = 0;
                        for (int n = -1; n <= 2; n++)
                        {
                            int sx = Math.Clamp(x1 + n, 0, srcW - 1);
                            double wx = CubicWeight(n - fx);
                            rowSum += src[sy * srcW + sx] * wx;
                        }
                        result += rowSum * wy;
                    }
                    dst[dstRow + x] = (byte)Math.Clamp(Math.Round(result), 0, 255);
                }
            });
            return dst;
        }

        private static double CubicWeight(double t)
        {
            t = Math.Abs(t);

            if (t <= 1.0)
            {
                return (1.5 * t - 2.5) * t * t + 1.0;
            }
            if (t < 2.0)
            {
                return ((-0.5 * t + 2.5) * t - 4.0) * t + 2.0;
            }
            return 0.0;
        }
        #endregion

        #endregion

        #region Notifications trigger
        private uint TriggerNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, bool autoDestroy = false)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                return mainWindow.SpawnNotification(text, notifType, autoDestroy);
            }
            return 0;
        }

        private void TriggerRemoveNotification(uint taskIdToRemove)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.RemoveNotification(taskIdToRemove);
            }
        }
        #endregion

        #region Image previews
        private void ShowCombinedPreview()
        {
            _isWaitingForCombinedImage = true; // waiting for it to finish

            if (_combinedPreviewCache == null)
            {
                return;
            }

            // image is valid, so show
            StartShowingCombinedPreview();
        }

        private void StartShowingCombinedPreview()
        {
            if (_isWaitingForCombinedImage)
            {
                _currentlyPreviewingType = CurrentBitmapPreviewingEnum.Combined;
                imgPreviewCombiner.ImageSource = _combinedPreviewCache;
                lblPreviewCombined.Text = $"Combined image";

                _isWaitingForCombinedImage = false;
            }
        }

        private void ShowChannelPreview(ColorChannelInput sender, ColorChannelEnum channel, bool isDirty = false)
        {
            bool isFromCombiner = sender.IsChannelFromCombined;

            // Check if the channel is already being previewed
            if (!isDirty && _currentlyPreviewingColorChannel == channel && _currentlyPreviewingType == CurrentBitmapPreviewingEnum.ColorChannel)
            {
                return;
            }

            if (ColorChannelHelper.GetCombinedChannelsDictionary()[sender.ColorChannel].Bitmap is not { } wantedImage)
            {
                return;
            }

            _currentlyPreviewingColorChannel = channel;
            _currentlyPreviewingType = CurrentBitmapPreviewingEnum.ColorChannel;
            lblPreviewCombined.Text = $"{channel} channel";

            imgPreviewCombiner.ImageSource = wantedImage;
        }

        private void ClearChannelPreview()
        {
            _currentlyPreviewingColorChannel = null;
            _currentlyPreviewingType = CurrentBitmapPreviewingEnum.None;
            lblPreviewCombined.Text = string.Empty;

            if (imgPreviewCombiner.ImageSource == null)
            {
                return;
            }
            imgPreviewCombiner.ImageSource = null;
        }
        #endregion

        #region Helpers

        private void UpdateTargetResolutionCombined()
        {
            var target = ColorChannelHelper.GetCombinedImageTargetResolution(_combinedDefaultSize);

            _resolutionCombinedOutputWidth = target.TargetOutputWidth;
            _resolutionCombinedOutputHeight = target.TargetOutputHeight;

            imgPreviewCombiner.Width = target.PreviewImageWidth;
            imgPreviewCombiner.Height = target.PreviewImageHeight;
        }
        #endregion
    }
}
