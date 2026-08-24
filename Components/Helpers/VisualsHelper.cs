using ImageCombinerChannelExtractor.Components.Enums;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class VisualsHelper
    {
        public static InfoBarSeverity GetSeverity(NotificationTypeEnum type) => type switch
        {
            NotificationTypeEnum.Error => InfoBarSeverity.Error,
            NotificationTypeEnum.Warning => InfoBarSeverity.Warning,

            NotificationTypeEnum.Success => InfoBarSeverity.Success,

            _ => InfoBarSeverity.Informational
        };
    }
}
