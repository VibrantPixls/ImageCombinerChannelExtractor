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
        public async static void SaveBitmapAsPNG(Button? sender, BitmapSource? bitmapToSave)
        {
            if (sender == null || bitmapToSave == null)
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

            var progress = new Progress<double>(percentage =>
            {
                App.MainWindowReference.SetExtractingScreenProgress(percentage);
            });

            await Task.Run(() =>
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapToSave));

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 65536, options: FileOptions.SequentialScan);
                using var progressStream = new FileExportingStreamer(fileStream, bytesWritten =>
                {
                    double percent = Math.Min(99.0, ((double)bytesWritten / estimatedTotalBytes) * 100.0);
                    ((IProgress<double>)progress).Report(percent);
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
