using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.PageClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.UserControls;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class ExtractorPage : ExtrPage
    {
        private CancellationTokenSource? _ctsExtract;

        public ExtractorPage()
        {
            InitializeComponent();

            _cachedColorInputPanels =
            [
                RedChannel,
                GreenChannel,
                BlueChannel,
                AlphaChannel
            ];

            UpdateLabel(StringLinesInfo.NoInputImageTextDefault);
        }

        #region On user events
        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelUserControl)sender;
            SetHoverOverChannel(input);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            ResetAllSelectedInputs();
        }

        private async void OnDownloadChannel(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelUserControl)sender;
            var bitmap = ColorChannelHelper.GetExtractedChannelBitmap(input.ColorChannel);

            if (bitmap == null)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                DefaultExt = ".png",
                FileName = "ExportedPreview.png",
                Title = "Save Preview As"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                bool gotError = false;
                var button = sender as Button;
                try
                {
                    if (button != null)
                    {
                        button.IsEnabled = false; // just to be sure
                    }
                    await DownloadHelper.SaveBitmapToPngAsync(saveFileDialog.FileName, bitmap);
                }
                catch (Exception ex)
                {
                    TriggerNotification(StringLinesInfo.GetExceptionError(ex), NotificationTypeEnum.Error, SharedInfo.NotificationAutoDestroyAfterInSecondsIfException);
                }
                finally
                {
                    if (button != null)
                    {
                        button.IsEnabled = true;
                    }

                    if (!gotError)
                    {
                        TriggerNotification(StringLinesInfo.notificationSuccessfullCombiningExport, NotificationTypeEnum.Success, SharedInfo.NotificationAutoDestroyAfterInSeconds);
                    }
                }
            }
        }

        private void OnButtonDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && files.Length > 0 && ColorChannelHelper.IsValidImageFile(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }
            e.Handled = true;
        }

        private void OnButtonDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                string? validFile = files?.FirstOrDefault(ColorChannelHelper.IsValidImageFile);
                if (!string.IsNullOrEmpty(validFile))
                {
                    UpdateLabel(Path.GetFileName(validFile));
                    ExtractFromPath(validFile);
                }
            }
        }
        #endregion

        public void UpdateLabel(string labelText)
        {
            lblResolutionCombined.Text = labelText;
        }

        #region On input image
        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            string? path = ColorChannelHelper.SelectPNGFile();
            ExtractFromPath(path);
        }

        private void Button_Click(object sender, RoutedEventArgs e) // delete input
        {
            _ctsExtract?.Cancel();

            ColorChannelHelper.DeleteChannel(this);
            imgPreviewCombiner.ImageSource = null;
            UpdateTargetResolution();
            ColorChannelHelper.ClearExtractedChannels();
            UpdateChannelPreviews();
        }
        #endregion

        #region Helpers
        private async void ExtractFromPath(string? path)
        {
            if (ColorChannelHelper.LoadChannelFromPath(this, path))
            {
                if (ColorChannelHelper.GetExtractedInput().Bitmap is not { } wantedImage)
                {
                    return;
                }
                imgPreviewCombiner.ImageSource = wantedImage;
                UpdateTargetResolution();

                await ExtractChannelsAsync();
            }
        }

        private void UpdateTargetResolution()
        {
            var target = ColorChannelHelper.GetTargetResolution();

            imgPreviewCombiner.Width = target.Width;
            imgPreviewCombiner.Height = target.Height;
        }

        private async Task ExtractChannelsAsync()
        {
            _ctsExtract?.Cancel();
            _ctsExtract?.Dispose();
            var cts = new CancellationTokenSource();
            _ctsExtract = cts;
            var token = cts.Token;

            var notifId = TriggerNotification(StringLinesInfo.notificationExtracting, NotificationTypeEnum.Info);
            try
            {
                bool extracted = await ColorChannelHelper.ExtractChannelsFromInputAsync(token);
                if (!extracted || token.IsCancellationRequested)
                {
                    return;
                }
                UpdateChannelPreviews();
                TriggerNotification(StringLinesInfo.notificationSuccessfullExtracting, NotificationTypeEnum.Success, SharedInfo.NotificationAutoDestroyAfterInSeconds);
            }
            catch (OperationCanceledException)
            {
                // was cancelled
            }
            catch (Exception ex)
            {
                TriggerNotification(StringLinesInfo.GetExceptionError(ex), NotificationTypeEnum.Error, SharedInfo.NotificationAutoDestroyAfterInSecondsIfException);
            }
            finally
            {
                TriggerRemoveNotification(notifId);
            }
        }

        private void UpdateChannelPreviews()
        {
            var extractedChannels = ColorChannelHelper.GetExtractedChannelsDictionary();

            foreach (var panel in _cachedColorInputPanels)
            {
                var outputPanel = (ColorChannelOutput)panel;

                var channelImage = extractedChannels[outputPanel.ColorChannel].Bitmap;
                outputPanel.SetPreview(channelImage);
                if (channelImage == null)
                {

                    outputPanel.SetLabelText($"{outputPanel.ColorChannel} channel");
                    continue;
                }
                else
                {
                    outputPanel.SetLabelText($"{outputPanel.ColorChannel} channel");
                }
            }
        }
        #endregion
    }
}
