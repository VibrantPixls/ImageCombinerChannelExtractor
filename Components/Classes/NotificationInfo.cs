using ImageCombinerChannelExtractor.Components.UserControls;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class NotificationInfo
    {
        public DoingTaskNotif Notif { get; }
        public DateTime TargetExpiration { get; }
        public bool IsTimed { get; }

        public NotificationInfo(DoingTaskNotif notif, int autoDestroyInSec)
        {
            Notif = notif;
            IsTimed = autoDestroyInSec > 0;
            if (IsTimed)
            {
                TargetExpiration = DateTime.UtcNow.AddSeconds(autoDestroyInSec);
            }
        }
    }
}
