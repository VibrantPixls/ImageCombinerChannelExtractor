using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public partial class CombExtrPage : Page
    {
        public required ReadOnlyMemory<ColorChannelUserControl> _cachedColorInputPanels;
        private ColorChannelUserControl? _currentlySelectedPanel;


        protected void ResetAllSelectedInputs()
        {
            _currentlySelectedPanel?.SetSelected(false);
            _currentlySelectedPanel = null;
        }

        protected void SetHoverOverChannel(ColorChannelUserControl newPreview)
        {
            if (ReferenceEquals(_currentlySelectedPanel, newPreview))
            {
                return;
            }
            _currentlySelectedPanel?.SetSelected(false);
            newPreview.SetSelected(true);
            _currentlySelectedPanel = newPreview;
        }
    }
}
