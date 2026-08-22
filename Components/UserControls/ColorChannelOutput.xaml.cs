using ImageCombinerChannelExtractor.Components.Classes.UserControlChildClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ColorChannelOutput : ColorChannelOutputClass
    {
        public ColorChannelOutput()
        {
            InitializeComponent();

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseEnterEvent, this));
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseLeaveEvent, this));
        }

        public void SetPreview(BitmapImage? image)
        {
            picboxPreview.ImageSource = image;
            btnDownloadChannel.Content = image != null ? StringLinesInfo.CrtExtractBtnDownload : StringLinesInfo.CrtExtractBtnNoInputs;
        }

        protected override void UpdateColoringStuff(ColorChannelEnum channel)
        {
            if (btnDownloadChannel is null)
            {
                return;
            }

            btnDownloadChannel.Content = StringLinesInfo.CrtExtractBtnNoInputs;
            base.UpdateColoringStuff(channel);
        }

        protected override void UpdateBrushColors(Brush wantedBrush)
        {
            base.UpdateBrushColors(wantedBrush);
            crdPanel.Background = wantedBrush;
        }
    }
}
