using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Settings;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => MainNavigation.Navigate(SettingsHelper.GetStartupPage());
            App.MainWindowReference.SetAppIcon(SettingsHelper.GetThemeMode() == ApplicationTheme.Light);
        }

        public void SetAppIcon(bool isLightMode)
        {
            if (TryFindResource(isLightMode ? "AppIconLightMode" : "AppIconDarkMode") is ImageSource newIcon)
            {
                appIconTitlbar.Source = newIcon;
            }
        }

        #region Extracting screen trigger
        public void ShowExtractingScreen(bool show)
        {
            loadingScreen.Visibility = show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void SetExtractingScreenProgress(double value)
        {
            loadingScreen.SetProgress(value);
        }
        #endregion

        #region Notifications trigger
        public uint SpawnNotification(string text, string description, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            if (!SettingsHelper.GetEnableNotifications())
            {
                return 0;
            }
            return notificationWidget.SpawnNotification(text, description, notifType, autoDestroyinSec);
        }

        public void RemoveNotification(uint taskIdToRemove)
        {
            if (taskIdToRemove == 0)
            {
                return;
            }
            notificationWidget.RemoveNotification(taskIdToRemove);
        }
        #endregion
    }
}
