using System.IO;

namespace ImageCombinerChannelExtractor.Components.Shared
{
    public static class SharedInfo
    {
        public static readonly string[] AllowedImageExtensions = { ".png", ".jpg", ".jpeg" };
        public static readonly string OpenFileDialogFilter;

        static SharedInfo()
        {
            var extsWithoutDot = AllowedImageExtensions.Select(ext => ext.TrimStart('.').ToLower()).ToArray();
            string combinedPattern = string.Join(";", extsWithoutDot.Select(ext => $"*.{ext}"));
            string individualFilters = string.Join("|", extsWithoutDot.Select(ext => $"{ext.ToUpper()} Files (*.{ext})|*.{ext}"));
            OpenFileDialogFilter = $"All Supported Images ({combinedPattern})|{combinedPattern}|{individualFilters}";
        }

        public static bool IsValidImageFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string extension = Path.GetExtension(filePath);
            return AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
