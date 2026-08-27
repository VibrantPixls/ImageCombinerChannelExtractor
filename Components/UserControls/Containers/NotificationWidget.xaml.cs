using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class NotificationWidget : UserControl
    {
        private readonly Dictionary<uint, NotificationInfo> _notifications = [];
        private readonly List<uint> _activeTimedIds = [];
        private readonly DispatcherTimer _timer;
        private uint _currentNotificationNumber = 0;

        public NotificationWidget()
        {
            InitializeComponent();

            _timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            DateTime now = DateTime.UtcNow;
            for (int i = _activeTimedIds.Count - 1; i >= 0; i--)
            {
                uint id = _activeTimedIds[i];
                if (!_notifications.TryGetValue(id, out NotificationInfo? state))
                {
                    _activeTimedIds.RemoveAt(i);
                    continue;
                }

                double remainingSeconds = (state.TargetExpiration - now).TotalSeconds;
                int secondsLeft = (int)Math.Ceiling(remainingSeconds);
                if (secondsLeft <= 0)
                {
                    state.Notif.SetSecondsLeft(0);
                    _activeTimedIds.RemoveAt(i);
                    RemoveNotificationInternal(id, state);
                }
                else
                {
                    state.Notif.SetSecondsLeft(secondsLeft);
                }
            }

            if (_activeTimedIds.Count == 0)
            {
                _timer.Stop();
            }
        }

        public uint SpawnNotification(string text, string description, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            uint notifId = GetCurrentNotificationInt();
            DoingTaskNotif notif = new DoingTaskNotif(notifType, text, description, notifId);
            notif.CloseRequested += OnNotificationCloseRequested;
            NotificationInfo state = new NotificationInfo(notif, autoDestroyinSec);

            _notifications.Add(notifId, state);
            NotificationContainer.Children.Insert(0, notif);
            scrlViewer.ScrollToEnd();

            if (state.IsTimed)
            {
                notif.SetSecondsLeft(autoDestroyinSec);
                _activeTimedIds.Add(notifId);

                if (!_timer.IsEnabled)
                {
                    _timer.Start();
                }
            }
            return notifId;
        }

        public void RemoveNotification(uint taskIdToRemove)
        {
            if (_notifications.TryGetValue(taskIdToRemove, out NotificationInfo? state))
            {
                if (state.IsTimed)
                {
                    _activeTimedIds.Remove(taskIdToRemove);
                }

                RemoveNotificationInternal(taskIdToRemove, state);
                if (_activeTimedIds.Count == 0 && _timer.IsEnabled)
                {
                    _timer.Stop();
                }
            }
        }

        #region Helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNotificationCloseRequested(uint notifId)
        {
            RemoveNotification(notifId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveNotificationInternal(uint id, NotificationInfo state)
        {
            state.Notif.CloseRequested -= OnNotificationCloseRequested;
            NotificationContainer.Children.Remove(state.Notif);
            _notifications.Remove(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetCurrentNotificationInt()
        {
            _currentNotificationNumber += 1;
            return _currentNotificationNumber;
        }
        #endregion
    }
}
