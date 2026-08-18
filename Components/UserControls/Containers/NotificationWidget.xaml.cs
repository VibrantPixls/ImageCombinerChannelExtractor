using ImageCombinerChannelExtractor.Components.Enums;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class NotificationWidget : UserControl
    {
        private readonly static Dictionary<uint, DoingTaskNotif> _notifications = new Dictionary<uint, DoingTaskNotif>();
        private static uint _currentNotificationNumber = 0;

        public NotificationWidget()
        {
            InitializeComponent();
        }

        public uint SpawnNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            var notifId = GetCurrentNotificationInt();
            var notif = new DoingTaskNotif(notifType, text);
            _notifications.Add(notifId, notif);
            NotificationContainer.Children.Insert(0, notif);

            if (autoDestroyinSec > 0)
            {
                AutoRemoveNotification(notifId, notif, autoDestroyinSec);
            }
            return notifId;
        }

        private async void AutoRemoveNotification(uint notifId, DoingTaskNotif notif, int amountOfSecondsToWait)
        {
            for (int i = amountOfSecondsToWait; i >= 0; i--)
            {
                notif.SetSecondsLeft(i);
                await Task.Delay(1000);
            }
            RemoveNotification(notifId);
        }

        public void RemoveNotification(uint taskIdToRemove)
        {
            if (!_notifications.TryGetValue(taskIdToRemove, out var notif))
            {
                return;
            }
            NotificationContainer.Children.Remove(notif);
            _notifications.Remove(taskIdToRemove);
        }

        #region Helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetCurrentNotificationInt()
        {
            _currentNotificationNumber += 1;
            return _currentNotificationNumber;
        }
        #endregion
    }
}
