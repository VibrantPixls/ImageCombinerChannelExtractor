using ImageCombinerChannelExtractor.Components.Classes.UserControlChildClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ColorChannelOutput : ColorChannelOutputClass
    {
        public ColorChannelOutput()
        {
            InitializeComponent();

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);
            SetLabelText(StringLinesInfo.NoInputImageTextDefault);
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseEnterEvent, this));
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseLeaveEvent, this));
        }

        protected override void UpdateColoringStuff(ColorChannelEnum channel)
        {
            if (btnDownloadChannel is null)
            {
                return;
            }

            btnDownloadChannel.Content = GetBtnString(channel);
            base.UpdateColoringStuff(channel);
        }

        protected override void UpdateBrushColors(Brush wantedBrush)
        {
            base.UpdateBrushColors(wantedBrush);
            crdPanel.Background = wantedBrush;
        }

        protected override string GetBtnString(ColorChannelEnum channel) => channel switch
        {
            _ => StringLinesInfo.CrtExtractBtnNoInputs
        };
    }
}
