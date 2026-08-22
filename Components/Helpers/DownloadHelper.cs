using ImageCombinerChannelExtractor.Components.Shared;
using ImageCombinerChannelExtractor.Components.Stream;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class DownloadHelper
    {
        public static async Task SaveBitmapToPngAsync(string filePath, BitmapSource bitmapToSave)
        {
            if (bitmapToSave == null)
            {
                return;
            }

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
