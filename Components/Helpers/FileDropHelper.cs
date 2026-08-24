using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public class FileDropHelper
    {
        private static string? IsValidFile(DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return null;
            }

            foreach (string file in files)
            {
                if (ColorChannelHelper.IsValidImageFile(file))
                {
                    return file;
                }
            }
            return null;
        }

        public static (bool isValid, string? validFile) IsFileValidAndReturnValidFile(DragEventArgs e)
        {
            string? validFile = IsValidFile(e);
            return (validFile != null, validFile);
        }

        public static bool IsFileValid(DragEventArgs e) => IsValidFile(e) != null;
        public static bool IsFileValidForDragOver(DragEventArgs e) => IsFileValid(e);
    }
}
