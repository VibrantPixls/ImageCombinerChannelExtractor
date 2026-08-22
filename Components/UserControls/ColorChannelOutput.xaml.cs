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

        public static readonly RoutedEvent ChannelDownloadClickEvent = EventManager.RegisterRoutedEvent(nameof(ChannelDownloadClickEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelOutputClass));
        public event RoutedEventHandler ChannelDownloadClick
        {
            add => AddHandler(ChannelDownloadClickEvent, value);
            remove => RemoveHandler(ChannelDownloadClickEvent, value);
        }

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

        public void SetPreview(BitmapImage? image)
        {
            picboxPreview.ImageSource = image;
            var isValid = image != null;
            btnDownloadChannel.Content = isValid ? StringLinesInfo.CrtExtractBtnDownload : StringLinesInfo.CrtExtractBtnNoInputs;
            btnDownloadChannel.IsEnabled = isValid;
            prgrRing.Visibility = Visibility.Hidden;
        }

        public void SetPreview()
        {
            picboxPreview.ImageSource = null;
            btnDownloadChannel.IsEnabled = false;
            prgrRing.Visibility = Visibility.Visible;
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
