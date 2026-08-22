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
    }
}
