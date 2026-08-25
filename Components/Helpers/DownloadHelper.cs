using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Stream;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class DownloadHelper
    {
        public async static void SaveBitmapAsPNG(Button? sender, BitmapSource? bitmapToSave, string fileName)
        {
            if (sender == null || bitmapToSave == null)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = SharedInfo.SaveFileDialogFilter,
                DefaultExt = SharedInfo.AllowedImageExtensionsSaving[0],
                FileName = $"{fileName}.png",
                Title = StringLinesInfo.SaveFileDialogTitle
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                bool gotError = false;
                try
                {
                    if (sender != null)
                    {
                        sender.IsEnabled = false; // just to be sure
                    }
                    await SaveBitmapToPngAsync(saveFileDialog.FileName, bitmapToSave);
                }
                catch (Exception ex)
                {
                    App.TriggerNotification(StringLinesInfo.GetExceptionError(ex), NotificationTypeEnum.Error, SharedInfo.NotificationAutoDestroyAfterInSecondsIfException);
                }
                finally
                {
                    if (sender != null)
                    {
                        sender.IsEnabled = true;
                    }

                    if (!gotError)
                    {
                        App.TriggerNotification(StringLinesInfo.notificationSuccessfullCombiningExport, NotificationTypeEnum.Success, SharedInfo.NotificationAutoDestroyAfterInSeconds);
                    }
                }
            }
        }

        private static async Task SaveBitmapToPngAsync(string filePath, BitmapSource bitmapToSave)
        {
            App.MainWindowReference.SetExtractingScreenProgress(0);
            App.MainWindowReference.ShowExtractingScreen(true);

            long estimatedTotalBytes = (long)(bitmapToSave.PixelWidth * bitmapToSave.PixelHeight * (bitmapToSave.Format.BitsPerPixel / 8) * 0.3);
            if (estimatedTotalBytes <= 0)
            {
                estimatedTotalBytes = 1;
            }

            Progress<double> progress = new Progress<double>(percentage =>
            {
                App.MainWindowReference.SetExtractingScreenProgress(percentage);
            });

            await Task.Run(() =>
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapToSave));

                using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 65536, options: FileOptions.SequentialScan);
                int lastReportedPercent = -1;
                using FileExportingStreamer progressStream = new FileExportingStreamer(fileStream, bytesWritten =>
                {
                    double percent = Math.Min(99.0, ((double)bytesWritten / estimatedTotalBytes) * 100.0);
                    int wholePercent = (int)percent;
                    if (wholePercent != lastReportedPercent)
                    {
                        lastReportedPercent = wholePercent;
                        ((IProgress<double>)progress).Report(wholePercent);
                    }
                });
                encoder.Save(progressStream);
            });
            App.MainWindowReference.SetExtractingScreenProgress(100);
            // delay after hitting 100%
            await Task.Delay(SharedInfo.OverlayKeepOnScreenAfterFinishForInMilliseconds).ConfigureAwait(true);
            App.MainWindowReference.ShowExtractingScreen(false);
        }
    }
}
