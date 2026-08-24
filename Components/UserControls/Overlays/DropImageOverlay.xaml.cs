using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class DropImageOverlay : UserControl
    {
        #region Variables
        public static readonly DependencyProperty ColorChannelProperty = DependencyProperty.Register(nameof(ColorChannel), typeof(ColorChannelEnum), typeof(DropImageOverlay), new PropertyMetadata(ColorChannelEnum.Red));
        public ColorChannelEnum ColorChannel
        {
            get => (ColorChannelEnum)GetValue(ColorChannelProperty);
            set => SetValue(ColorChannelProperty, value);
        }
        #endregion

        public DropImageOverlay()
        {
            InitializeComponent();

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);
        }

        private void UpdateColoringStuff(ColorChannelEnum colorChannel)
        {
            bgrPnl.Background = ColorHelper.GetColorBrush(colorChannel, ColorBrushTypeEnum.Opaque);
        }
    }
}
