using ImageCombinerChannelExtractor.Components.Settings.Enums;
using Wpf.Ui.Appearance;

namespace ImageCombinerChannelExtractor.Components.Settings
{
    public class SettingsClass
    {
        public ApplicationTheme ColorThemeMode { get; set; } = ApplicationTheme.Dark;
        public StartupPageEnum StartupPage { get; set; } = StartupPageEnum.Combiner;
    }
}
