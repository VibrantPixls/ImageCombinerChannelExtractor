using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public class FileDropHelper
    {
        private static string? _lastCheckedFilePath;
        private static string? _cachedValidFilePath;

        private static string? IsValidFile(DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                _lastCheckedFilePath = null;
                _cachedValidFilePath = null;
                return null;
            }

            string firstFile = files[0];
            if (string.Equals(_lastCheckedFilePath, firstFile, StringComparison.OrdinalIgnoreCase))
            {
                return _cachedValidFilePath;
            }

            string? validFile = null;
            var isValid = ColorChannelHelper.IsValidImageFile(firstFile);
            if (isValid)
            {
                validFile = firstFile;
            }
            _lastCheckedFilePath = firstFile;
            _cachedValidFilePath = isValid ? firstFile : null;
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
