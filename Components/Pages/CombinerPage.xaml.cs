using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.PageClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Structs;
using ImageCombinerChannelExtractor.Components.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class CombinerPage : CombPage
    {
        #region Image previews variables
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
        private uint successNotifInt = 0;
        #endregion

        public CombinerPage()
        {
            InitializeComponent();

            _cachedColorInputPanels =
            [
                RedChannelInput,
                GreenChannelInput,
                BlueChannelInput,
                AlphaChannelInput
            ];

            brdPreviewBoxCombined.Width = SharedInfo.CombinedPreviewDefaultSize;
            brdPreviewBoxCombined.Height = SharedInfo.CombinedPreviewDefaultSize;

            btnCreateCombined.Content = StringLinesInfo.CrtCombinedBtnNoInputs;
        }

        #region On ColorChannelInput events
        private void OnChannelClick(object sender, FileSelectedEventArgs e)
        {
            ColorChannelInput input = (ColorChannelInput)sender;
            input.SetDraggingOver(false);
            string? path = e.SelectedFilePath ?? ColorChannelHelper.SelectPNGFile();
            LoadChannelFromPath(input, path);
        }

        private void OnChannelClickRemove(object sender, RoutedEventArgs e)
        {
            ColorChannelInput input = (ColorChannelInput)sender;
            DeleteChannel(input);
        }

        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            ColorChannelInput input = (ColorChannelInput)sender;
            ShowChannelPreview(input);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            ResetAllSelectedInputs();
        }

        private void OnButtonMouseEnterCombined(object sender, MouseEventArgs e)
        {
            ShowCombinedPreview();
        }

        private void OnFilteringChanged(object sender, RoutedEventArgs e)
        {
            ColorChannelInput input = (ColorChannelInput)sender;
            if (!ColorChannelHelper.DoesSenderInputChannelHaveInputImageForCombined(input.ColorChannel)) // should pass for pixel arts
            {
                return;
            }
            ColorChannelHelper.OnFilteringChanged(input);

            MarkSenderPreviewDirty(input.ColorChannel);
            MarkCombinedPreviewDirty();
        }

        private async void btnCreateCombined_Click(object sender, RoutedEventArgs e) // export file
        {
            DownloadHelper.SaveBitmapAsPNG(sender as Button, _combinedPreviewCache, StringLinesInfo.DownloadImgFileNameCombined);
        }
        #endregion

        #region Load images into channels
        private void LoadChannelFromPath(ColorChannelInput sender, string? path)
        {
            if (!ColorChannelHelper.DoesSenderInputChannelHaveDifferentImageForCombined(sender.ColorChannel, path))
            {
                return;
            }

            bool successfullyLoaded = ColorChannelHelper.LoadChannelFromPath(sender, path);
            if (successfullyLoaded)
            {
                ShowChannelPreview(sender, true);
            }
            UpdateResolution();

            MarkCombinedPreviewDirty();

            sender.UpdateDeleteButtonEnabled(successfullyLoaded);
        }

        private void DeleteChannel(ColorChannelInput sender)
        {
            ColorChannelHelper.DeleteChannel(sender);

            ClearChannelPreview();
            UpdateResolution();

            MarkCombinedPreviewDirty();

            sender.UpdateDeleteButtonEnabled(false);
        }

        private void UpdateResolution()
        {
            byte filledChannels = ColorChannelHelper.GetFilledCombinedColorChannelsAmount();
            if (filledChannels == 0)
            {
                _differentResolutionsForCombinedOutput = false;
                _resolutionCombinedOutputWidth = 0;
                _resolutionCombinedOutputHeight = 0;
                lblResolutionCombined.Text = string.Empty;

                brdPreviewBoxCombined.Width = SharedInfo.CombinedPreviewDefaultSize;
                brdPreviewBoxCombined.Height = SharedInfo.CombinedPreviewDefaultSize;
                return;
            }

            _differentResolutionsForCombinedOutput = ColorChannelHelper.AreCombinedChannelsMismatchedInSize();
            UpdateTargetResolutionCombined();

            lblResolutionCombined.Text = _differentResolutionsForCombinedOutput ? $"Mismatched sizes - output will be {_resolutionCombinedOutputWidth}x{_resolutionCombinedOutputHeight}" : $"{_resolutionCombinedOutputWidth}x{_resolutionCombinedOutputHeight}";
        }

        private void UpdateFilteringComboboxEnable()
        {
            foreach (ColorChannelUserControl panel in _cachedColorInputPanels)
            {
                ColorChannelInput converted = (ColorChannelInput)panel;
                bool hasInputImage = ColorChannelHelper.DoesSenderInputChannelHaveInputImageForCombined(converted.ColorChannel);
                // should be a function, oh well
                converted.cmbboxFiltering.IsEnabled = hasInputImage; // small images should be able to set filtering for pixel-art's
            }
        }
        #endregion

        #region Combiner
        public void MarkCombinedPreviewDirty()
        {
            UpdateFilteringComboboxEnable();

            _combinedPreviewCache = null;
            _ = GenerateCombinedPreviewAsync();
        }

        private async Task GenerateCombinedPreviewAsync()
        {
            btnCreateCombined.IsEnabled = false;
            if (!ColorChannelHelper.DoesAnyChannelHaveInputImagesForCombined())
            {
                //early exit when no input images
                btnCreateCombined.Content = StringLinesInfo.CrtCombinedBtnNoInputs;
                return;
            }

            btnCreateCombined.Content = StringLinesInfo.CrtCombinedBtnGenerating;

            _ctsCombined?.Cancel();
            _ctsCombined?.Dispose();
            CancellationTokenSource cts = new CancellationTokenSource();
            _ctsCombined = cts;
            CancellationToken token = cts.Token;

            if (successNotifInt != 0)
            {
                App.TriggerRemoveNotification(successNotifInt);
            }

            uint notifInt = App.TriggerNotification(StringLinesInfo.notificationCombining, NotificationTypeEnum.Combining);
            try
            {
                //await Task.Delay(10, token).ConfigureAwait(true);

                // Snapshot it before doing anything
                Dictionary<ColorChannelEnum, ChannelSlot> snapshot = new Dictionary<ColorChannelEnum, ChannelSlot>(ColorChannelHelper.GetCombinedChannelsDictionary());

                BitmapSource? result = await Task.Run(() => BuildCombinedImage(snapshot, token), token).ConfigureAwait(true);

                if (!token.IsCancellationRequested)
                {
                    _combinedPreviewCache = result;
                    // now show if the user was waiting for it
                    StartShowingCombinedPreview();

                    btnCreateCombined.IsEnabled = true;
                    btnCreateCombined.Content = StringLinesInfo.CrtCombinedBtn;

                    successNotifInt = App.TriggerNotification(StringLinesInfo.notificationSuccessfullCombining, NotificationTypeEnum.Success, SharedInfo.NotificationAutoDestroyAfterInSeconds);
                }
            }
            catch (OperationCanceledException)
            {
                // cancelled
            }
            finally
            {
                App.TriggerRemoveNotification(notifInt);
            }
        }

        private static BitmapSource? BuildCombinedImage(Dictionary<ColorChannelEnum, ChannelSlot> channels, CancellationToken token)
        {
            Dictionary<ColorChannelEnum, ChannelSlot> sources = channels.Where(kv => kv.Value?.Bitmap != null).ToDictionary(kv => kv.Key, kv => kv.Value!);
            if (sources.Count == 0)
            {
                return null;
            }

            token.ThrowIfCancellationRequested();

            byte[]? r = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.red), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? g = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.green), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? b = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.blue), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);
            byte[]? a = GetChannelGrayscaleBuffer(sources.GetValueOrDefault(ColorChannelEnum.alpha), _resolutionCombinedOutputWidth, _resolutionCombinedOutputHeight, token);

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

            BitmapSource source = image;
            int nativeW = source.PixelWidth;
            int nativeH = source.PixelHeight;

            FormatConvertedBitmap grayscaleConverter = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            byte[] nativeBuffer = new byte[nativeW * nativeH];
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

            byte[] canvas = new byte[targetWidth * targetHeight];
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

            byte[] dst = new byte[dstW * dstH];
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

            byte[] dst = new byte[dstW * dstH];
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

            byte[] dst = new byte[dstW * dstH];
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

        private void MarkSenderPreviewDirty(ColorChannelEnum channel)
        {
            RenderOptions.SetBitmapScalingMode(imgPreviewCombiner, ColorChannelHelper.GetBitmapScalingFilteringMode(channel));
        }

        private void StartShowingCombinedPreview()
        {
            if (_isWaitingForCombinedImage)
            {
                ResetAllSelectedInputs();

                RenderOptions.SetBitmapScalingMode(imgPreviewCombiner, BitmapScalingMode.HighQuality);
                _currentlyPreviewingType = CurrentBitmapPreviewingEnum.Combined;
                imgPreviewCombiner.ImageSource = _combinedPreviewCache;
                lblPreviewCombined.Text = $"Combined image";

                _isWaitingForCombinedImage = false;
            }
        }

        private void ShowChannelPreview(ColorChannelInput sender, bool isDirty = false)
        {
            ColorChannelEnum channel = sender.ColorChannel;

            SetHoverOverChannel(sender);

            // Check if the channel is already being previewed
            if (!isDirty && _currentlyPreviewingColorChannel == channel && _currentlyPreviewingType == CurrentBitmapPreviewingEnum.ColorChannel)
            {
                return;
            }

            if (ColorChannelHelper.GetCombinedChannelsDictionary()[channel].Bitmap is not { } wantedImage)
            {
                return;
            }

            _currentlyPreviewingColorChannel = channel;
            _currentlyPreviewingType = CurrentBitmapPreviewingEnum.ColorChannel;
            lblPreviewCombined.Text = $"{channel} channel";

            RenderOptions.SetBitmapScalingMode(imgPreviewCombiner, ColorChannelHelper.GetBitmapScalingFilteringMode(channel));
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

            // reset preview back to default size
            imgPreviewCombiner.Width = SharedInfo.CombinedPreviewDefaultSize;
            imgPreviewCombiner.Height = SharedInfo.CombinedPreviewDefaultSize;
        }
        #endregion

        #region Helpers
        private void UpdateTargetResolutionCombined()
        {
            CombinedImageTargetStruct target = ColorChannelHelper.GetCombinedImageTargetResolution();

            _resolutionCombinedOutputWidth = target.TargetOutputWidth;
            _resolutionCombinedOutputHeight = target.TargetOutputHeight;

            imgPreviewCombiner.Width = target.PreviewImageWidth;
            imgPreviewCombiner.Height = target.PreviewImageHeight;
        }
        #endregion
    }
}
