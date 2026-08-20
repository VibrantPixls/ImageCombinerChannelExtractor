using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.PageClasses;
using ImageCombinerChannelExtractor.Components.Helpers;
using System.Diagnostics;
using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class ExtractorPage : ExtrPage
    {
        private string? _selectedFilePath = null;

        public ExtractorPage()
        {
            InitializeComponent();

            _cachedColorInputPanels = new[]
            {
                RedChannel,
                GreenChannel,
                BlueChannel,
                AlphaChannel
            };
        }

        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelUserControl)sender;
            Debug.WriteLine($"OnChannelMouseEnter on {input.ColorChannel}");
            SetHoverOverChannel(input);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            ResetAllSelectedInputs();
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
