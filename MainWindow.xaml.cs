using ImageCombinerChannelExtractor.Components.Enums;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Notifications trigger
        public uint SpawnNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, bool autoDestroy = false)
        {
            return notificationWidget.SpawnNotification(text, notifType, autoDestroy);
        }

        public void RemoveNotification(uint taskIdToRemove)
        {
            notificationWidget.RemoveNotification(taskIdToRemove);
        }
        #endregion
    }
}
