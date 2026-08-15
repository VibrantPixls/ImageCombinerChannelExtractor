using ImageCombinerChannelExtractor.Components.Enums;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    // find the icons here: 
    // https://learn.microsoft.com/en-us/windows/apps/design/iconography/segoe-ui-symbol-font
    public static class WindowsIconHelper
    {
        public static string GetWindowsIcon(NotificationTypeEnum type) => type switch
        {
            NotificationTypeEnum.Error => "\uE783",
            NotificationTypeEnum.Warning => "\uE7BA",

            NotificationTypeEnum.Combining => "\uE8C8",

            NotificationTypeEnum.Success => "\uE76E",

            _ => "\uE82F" // info
        };
    }
}
