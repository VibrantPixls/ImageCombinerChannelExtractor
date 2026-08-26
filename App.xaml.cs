using ImageCombinerChannelExtractor.Components.Enums;
using System.Windows;

namespace ImageCombinerChannelExtractor
{
    public partial class App : Application
    {
        public static MainWindow MainWindowReference => (MainWindow)Current.MainWindow;

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
