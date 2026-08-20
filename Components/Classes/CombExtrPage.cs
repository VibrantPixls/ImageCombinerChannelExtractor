using ImageCombinerChannelExtractor.Components.Enums;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public partial class CombExtrPage : Page
    {
        public required ColorChannelUserControl[] _cachedColorInputPanels;

        protected void ResetAllSelectedInputs()
        {
            foreach (var panel in _cachedColorInputPanels)
            {
                panel.SetSelected(false);
            }
        }

        protected void SetHoverOverChannel(ColorChannelUserControl newPreview)
        {
            ResetAllSelectedInputs();
            newPreview.SetSelected(true);
        }

        #region Notifications trigger
        protected uint TriggerNotification(string text, NotificationTypeEnum notifType = NotificationTypeEnum.Info, int autoDestroyinSec = 0)
        {
            return App.MainWindowReference.SpawnNotification(text, notifType, autoDestroyinSec);
        }

        protected void TriggerRemoveNotification(uint taskIdToRemove)
        {
            App.MainWindowReference.RemoveNotification(taskIdToRemove);
        }
        #endregion
    }
}
