using ImageCombinerChannelExtractor.Components.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class ExtractorPage : Page
    {
        private string? _selectedFilePath = null;

        public ExtractorPage()
        {
            InitializeComponent();
        }

        public void UpdateLabel(string labelText)
        {
            lblResolutionCombined.Text = labelText;
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            _selectedFilePath = ColorChannelHelper.SelectPNGFile();
            if (ColorChannelHelper.LoadChannelFromPath(this, _selectedFilePath))
            {
                if (ColorChannelHelper.GetExtractedInput().Bitmap is not { } wantedImage)
                {
                    return;
                }

                imgPreviewCombiner.ImageSource = wantedImage;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _selectedFilePath = null;
            ColorChannelHelper.DeleteChannel(this);
            imgPreviewCombiner.ImageSource = null;
        }
    }
}
