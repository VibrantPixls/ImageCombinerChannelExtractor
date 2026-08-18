using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ExportingScreen : UserControl
    {
        public ExportingScreen()
        {
            InitializeComponent();
        }

        public void SetProgress(double value)
        {
            progressbar.Value = value;
        }
    }
}
