using ImageCombinerChannelExtractor.Components.Classes.UserControlChildClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
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
            SetImageResolution(SharedInfo.ExtractorPreviewOutputDefaultSize);

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);
        }

        #region Variables
        public static readonly RoutedEvent ChannelDownloadClickEvent = EventManager.RegisterRoutedEvent(nameof(ChannelDownloadClickEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelOutputClass));
        public event RoutedEventHandler ChannelDownloadClick
        {
            add => AddHandler(ChannelDownloadClickEvent, value);
            remove => RemoveHandler(ChannelDownloadClickEvent, value);
        }
        #endregion

        #region User input
        private void btnDownloadChannel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelDownloadClickEvent, this));
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseEnterEvent, this));
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseLeaveEvent, this));
        }
        #endregion

        public void SetPreview(BitmapImage? image)
        {
            picboxPreview.ImageSource = image;
            bool isValid = image != null;
            if (isValid)
            {
#pragma warning disable CS8604
                (double Width, double Height) res = ColorChannelHelper.GetExtractorOutputImageTargetResolution(image);
#pragma warning restore CS8604
                SetImageResolution(res.Width, res.Height);
            }
            else
            {
                SetImageResolution(SharedInfo.ExtractorPreviewOutputDefaultSize);
            }
            btnDownloadChannel.Content = isValid ? StringLinesInfo.GetCrtExtractBtnDownload(ColorChannel) : StringLinesInfo.CrtExtractBtnNoInputs;
            btnDownloadChannel.IsEnabled = isValid;
            prgrRing.Visibility = Visibility.Hidden;
        }

        public void SetPreview()
        {
            picboxPreview.ImageSource = null;
            SetImageResolution(SharedInfo.ExtractorPreviewOutputDefaultSize);
            btnDownloadChannel.IsEnabled = false;
            prgrRing.Visibility = Visibility.Visible;
        }

        private void SetImageResolution(double size)
        {
            SetImageResolution(size, size);
        }

        private void SetImageResolution(double width, double height)
        {
            picboxPreview.Width = width;
            picboxPreview.Height = height;
        }

        #region Overrides
        protected override void UpdateColoringStuff(ColorChannelEnum channel)
        {
            btnDownloadChannel.Content = picboxPreview.ImageSource != null ? StringLinesInfo.GetCrtExtractBtnDownload(channel) : StringLinesInfo.CrtExtractBtnNoInputs;
            base.UpdateColoringStuff(channel);
        }

        protected override void UpdateBrushColors(Brush wantedBrush)
        {
            base.UpdateBrushColors(wantedBrush);
            crdPanel.Background = wantedBrush;
        }
        #endregion
    }
}
