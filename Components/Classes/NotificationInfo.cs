using ImageCombinerChannelExtractor.Components.UserControls;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class NotificationInfo
    {
        public NotificationUserControl Notif { get; }
        public DateTime TargetExpiration { get; }
        public bool IsTimed { get; }

        public NotificationInfo(NotificationUserControl notif, int autoDestroyInSec)
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
