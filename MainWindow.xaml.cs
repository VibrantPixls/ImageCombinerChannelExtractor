using ImageCombinerChannelExtractor.Components.Enums;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => RootNavigation.Navigate(typeof(Components.Pages.CombinerPage));
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
        public uint SpawnNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            return notificationWidget.SpawnNotification(text, notifType, autoDestroyinSec);
        }

        public void RemoveNotification(uint taskIdToRemove)
        {
            notificationWidget.RemoveNotification(taskIdToRemove);
        }
        #endregion
    }
}
