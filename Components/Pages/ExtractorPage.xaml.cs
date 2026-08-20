using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.PageClasses;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Diagnostics;
using System.IO;
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

            UpdateLabel(StringLinesInfo.NoInputImageTextDefault);
        }

        #region On ColorChannelInput events
        #endregion

        #region On mouse events
        private void OnChannelMouseEnter(object sender, RoutedEventArgs e)
        {
            var input = (ColorChannelUserControl)sender;
            SetHoverOverChannel(input);
        }

        private void OnChannelMouseLeave(object sender, RoutedEventArgs e)
        {
            ResetAllSelectedInputs();
        }

        private void OnButtonDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && files.Length > 0 && ColorChannelHelper.IsValidImageFile(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }
            e.Handled = true;
        }

        private void OnButtonDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                string? validFile = files?.FirstOrDefault(ColorChannelHelper.IsValidImageFile);
                if (!string.IsNullOrEmpty(validFile))
                {
                    UpdateLabel(Path.GetFileName(validFile));
                    ExtractFromPath(validFile);
                }
            }
        }
        #endregion

        public void UpdateLabel(string labelText)
        {
            lblResolutionCombined.Text = labelText;
        }

        #region On input image
        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            string? path = ColorChannelHelper.SelectPNGFile();
            ExtractFromPath(path);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _selectedFilePath = null;
            ColorChannelHelper.DeleteChannel(this);
            imgPreviewCombiner.ImageSource = null;
            UpdateTargetResolution();
        }
        #endregion

        #region Helpers
        private void ExtractFromPath(string? path)
        {
            _selectedFilePath = path;
            if (ColorChannelHelper.LoadChannelFromPath(this, _selectedFilePath))
            {
                if (ColorChannelHelper.GetExtractedInput().Bitmap is not { } wantedImage)
                {
                    return;
                }
                imgPreviewCombiner.ImageSource = wantedImage;
                UpdateTargetResolution();
            }
        }

        private void UpdateTargetResolution()
        {
            var target = ColorChannelHelper.GetTargetResolution();

            imgPreviewCombiner.Width = target.Width;
            imgPreviewCombiner.Height = target.Height;
        }
        #endregion
    }
}
