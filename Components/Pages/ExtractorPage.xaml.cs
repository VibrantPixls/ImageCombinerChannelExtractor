using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.PageClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Interfaces;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.UserControls;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class ExtractorPage : ExtrPage, DragOverInterface
    {
        private CancellationTokenSource? _ctsExtract;

        public ExtractorPage()
        {
            InitializeComponent();

            DragOverInterface dragHandler = this;
            crdPanel.DragEnter += dragHandler.DraggingIntoWindow;
            crdPanel.DragLeave += dragHandler.DraggingLeaveWindow;

            _cachedColorInputPanels = new[]
            {
                RedChannel,
                GreenChannel,
                BlueChannel,
                AlphaChannel
            };

            UpdateLabel(StringLinesInfo.NoInputImageTextDefault);
        }

        #region On user events
        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            ColorChannelUserControl input = (ColorChannelUserControl)sender;
            SetHoverOverChannel(input);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            ResetAllSelectedInputs();
        }

        private async void OnDownloadChannel(object sender, RoutedEventArgs e)
        {
            ColorChannelOutput input = (ColorChannelOutput)sender;
            ColorChannelEnum channel = input.ColorChannel;
            DownloadHelper.SaveBitmapAsPNG(input.btnDownloadChannel, ColorChannelHelper.GetExtractedChannelBitmap(channel), StringLinesInfo.GetDownloadImgFileNameExtractedChannel(channel));
        }

        private void OnButtonDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if (FileDropHelper.IsFileValid(e))
            {
                e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }

        private void OnButtonDrop(object sender, DragEventArgs e)
        {
            SetDraggingOver(false);
            (bool isValid, string? validFile) result = FileDropHelper.IsFileValidAndReturnValidFile(e);
            if (result.isValid)
            {
#pragma warning disable CS8604
                UpdateLabel(Path.GetFileName(result.validFile));
#pragma warning restore CS8604
                ExtractFromPath(result.validFile);
            }
        }
        #endregion

        public void UpdateLabel(string labelText)
        {
            lblResolutionCombined.Text = labelText;
        }

        #region Interfaces
        public void SetDraggingOver(bool draggingOver)
        {
            dropOvrl.Visibility = draggingOver ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion

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
            if (!ColorChannelHelper.LoadChannelFromPath(this, path) || ColorChannelHelper.GetExtractedInput().Bitmap is not { } wantedImage)
            {
                btnDeleteInputExtract.IsEnabled = false;
                return;
            }
            imgPreviewCombiner.ImageSource = wantedImage;
            btnDeleteInputExtract.IsEnabled = true;
            UpdateTargetResolution();
            await ExtractChannelsAsync();
        }

        private void UpdateTargetResolution()
        {
            (double Width, double Height) target = ColorChannelHelper.GetTargetResolution();

            Debug.WriteLine($"combiner width - {target.Width} x {target.Height}");

            imgPreviewCombiner.Width = target.Width;
            imgPreviewCombiner.Height = target.Height;
        }

        private async Task ExtractChannelsAsync()
        {
            _ctsExtract?.Cancel();
            _ctsExtract?.Dispose();
            CancellationTokenSource cts = new CancellationTokenSource();
            _ctsExtract = cts;
            CancellationToken token = cts.Token;

            uint notifId = App.TriggerNotification(StringLinesInfo.notificationExtracting, NotificationTypeEnum.Info);
            try
            {
                UpdateChannelPreviews(true);
                //await Task.Delay(10000, token).ConfigureAwait(true);
                bool extracted = await ColorChannelHelper.ExtractChannelsFromInputAsync(token);
                if (!extracted || token.IsCancellationRequested)
                {
                    return;
                }
                UpdateChannelPreviews();
                App.TriggerNotification(StringLinesInfo.notificationSuccessfullExtracting, NotificationTypeEnum.Success, SharedInfo.NotificationAutoDestroyAfterInSeconds);
            }
            catch (OperationCanceledException)
            {
                // was cancelled
            }
            catch (Exception ex)
            {
                App.TriggerNotification(StringLinesInfo.GetExceptionError(ex), NotificationTypeEnum.Error, SharedInfo.NotificationAutoDestroyAfterInSecondsIfException);
            }
            finally
            {
                App.TriggerRemoveNotification(notifId);
            }
        }

        private void UpdateChannelPreviews(bool isLoading = false)
        {
            Dictionary<ColorChannelEnum, ChannelSlot> extractedChannels = ColorChannelHelper.GetExtractedChannelsDictionary();
            foreach (ColorChannelUserControl panel in _cachedColorInputPanels)
            {
                ColorChannelOutput outputPanel = (ColorChannelOutput)panel;

                if (isLoading)
                {
                    outputPanel.SetPreview();
                    continue;
                }

                BitmapImage? channelImage = extractedChannels[outputPanel.ColorChannel].Bitmap;
                outputPanel.SetPreview(channelImage);
                if (channelImage == null)
                {

                    outputPanel.SetLabelText($"{outputPanel.ColorChannel} channel");
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
