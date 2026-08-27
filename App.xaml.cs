using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Settings;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor
{
    public partial class App : Application
    {
        public static MainWindow MainWindowReference => (MainWindow)Current.MainWindow;

        #region Settings
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplySavedSettings();
        }

        private static void ApplySavedSettings()
        {
            ApplyTheme(SettingsHelper.GetThemeMode());
        }

        #region App settings helpers
        public static void ApplyTheme(ApplicationTheme theme)
        {
            bool isLightTheme = (theme == ApplicationTheme.Light);
            SharedInfo.UpdateColorBrushes(isLightTheme);
            ApplicationAccentColorManager.Apply(isLightTheme ? SharedInfo.ApplicationAccentColorLight : SharedInfo.ApplicationAccentColorDark, theme);
            ApplicationThemeManager.Apply(theme, WindowBackdropType.None, false);
        }
        #endregion
        #endregion

        #region Notifications trigger
        public static uint TriggerNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            return TriggerNotification(text, string.Empty, notifType, autoDestroyinSec);
        }

        public static uint TriggerNotification(string text, string description, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            return MainWindowReference.SpawnNotification(text, description, notifType, autoDestroyinSec);
        }

        public static void TriggerRemoveNotification(uint taskIdToRemove)
        {
            MainWindowReference.RemoveNotification(taskIdToRemove);
        }
        #endregion
    }
}
