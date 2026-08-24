using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public class FileDropHelper
    {
        private static string? _cachedFilePath;

        private static string? IsValidFile(DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                _cachedFilePath = null;
                return null;
            }

            string firstFile = files[0];
            if (string.Equals(_cachedFilePath, firstFile, StringComparison.OrdinalIgnoreCase))
            {
                return _cachedFilePath;
            }

            string? validFile = null;
            if (ColorChannelHelper.IsValidImageFile(firstFile))
            {
                validFile = firstFile;
            }
            _cachedFilePath = firstFile;
            return validFile;
        }

        public static (bool isValid, string? validFile) IsFileValidAndReturnValidFile(DragEventArgs e)
        {
            string? validFile = IsValidFile(e);
            return (validFile != null, validFile);
        }

        public static bool IsFileValid(DragEventArgs e) => IsValidFile(e) != null;
    }
}
