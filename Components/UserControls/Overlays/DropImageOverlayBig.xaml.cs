using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class DropImageOverlayBig : UserControl
    {
        public DropImageOverlayBig()
        {
            InitializeComponent();
            bgrPnl.Background = ColorHelper.GetColorBrushForMaster();
        }
    }
}
