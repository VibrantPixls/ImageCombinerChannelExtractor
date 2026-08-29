using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Settings;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            OverrideControlStyle<Wpf.Ui.Controls.Button>(style =>
            {
                style.Setters.Add(new Setter(FrameworkElement.CursorProperty, Cursors.Hand));
                style.Setters.Add(new Setter(Wpf.Ui.Controls.Button.ClickModeProperty, ClickMode.Press));
            });
            OverrideControlStyle<ComboBox>(style =>
            {
                style.Setters.Add(new Setter(FrameworkElement.CursorProperty, Cursors.Hand));
            });
            OverrideControlStyle<Anchor>(style =>
            {
                style.Setters.Add(new Setter(FrameworkElement.CursorProperty, Cursors.Hand));
            });

            ApplySavedSettings();
        }

        private static void OverrideControlStyle<T>(Action<Style> configureStyle) where T : FrameworkElement
        {
            if (Current.TryFindResource(typeof(T)) is Style baseStyle)
            {
                Style updatedStyle = new Style(typeof(T), baseStyle);
                configureStyle(updatedStyle);
                updatedStyle.Seal();
                Current.Resources[typeof(T)] = updatedStyle;
            }
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
