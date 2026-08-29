using ImageCombinerChannelExtractor.Components.Settings.Enums;
using Wpf.Ui.Appearance;

namespace ImageCombinerChannelExtractor.Components.Settings
{
    public class SettingsClass
    {
        // { get; set; } is needed for json to work
        public ApplicationTheme ColorThemeMode { get; set; } = ApplicationTheme.Dark;
        public StartupPageEnum StartupPage { get; set; } = StartupPageEnum.Combiner;
        public bool EnableNotifications { get; set; } = true;
        public bool DoExtractFlicker { get; set; } = false;
    }
}
