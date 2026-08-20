using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public partial class ColorChannelUserControl : System.Windows.Controls.UserControl
    {
        #region Variables
        public static readonly DependencyProperty ColorChannelProperty = DependencyProperty.Register(nameof(ColorChannel), typeof(ColorChannelEnum), typeof(ColorChannelUserControl), new PropertyMetadata(ColorChannelEnum.Red));
        public ColorChannelEnum ColorChannel
        {
            get => (ColorChannelEnum)GetValue(ColorChannelProperty);
            set => SetValue(ColorChannelProperty, value);
        }

        public static readonly DependencyProperty IsChannelFromCombinedProperty = DependencyProperty.Register(nameof(IsChannelFromCombined), typeof(bool), typeof(ColorChannelUserControl), new PropertyMetadata(true));
        public bool IsChannelFromCombined
        {
            get => (bool)GetValue(IsChannelFromCombinedProperty);
            set => SetValue(IsChannelFromCombinedProperty, value);
        }
        #endregion

        #region Mouse events
        public static readonly RoutedEvent ChannelClickEvent = EventManager.RegisterRoutedEvent(nameof(ChannelClick), RoutingStrategy.Bubble, typeof(EventHandler<FileSelectedEventArgs>), typeof(ColorChannelUserControl));
        public event EventHandler<FileSelectedEventArgs> ChannelClick
        {
            add => AddHandler(ChannelClickEvent, value);
            remove => RemoveHandler(ChannelClickEvent, value);
        }

        public static readonly RoutedEvent ChannelClickRemoveEvent = EventManager.RegisterRoutedEvent(nameof(ChannelClickRemove), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelUserControl));
        public event RoutedEventHandler ChannelClickRemove
        {
            add => AddHandler(ChannelClickRemoveEvent, value);
            remove => RemoveHandler(ChannelClickRemoveEvent, value);
        }

        public static readonly RoutedEvent ChannelMouseEnterEvent = EventManager.RegisterRoutedEvent(nameof(ChannelMouseEnter), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelUserControl));
        public event RoutedEventHandler ChannelMouseEnter
        {
            add => AddHandler(ChannelMouseEnterEvent, value);
            remove => RemoveHandler(ChannelMouseEnterEvent, value);
        }

        public static readonly RoutedEvent ChannelMouseLeaveEvent = EventManager.RegisterRoutedEvent(nameof(ChannelMouseLeave), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelUserControl));
        public event RoutedEventHandler ChannelMouseLeave
        {
            add => AddHandler(ChannelMouseLeaveEvent, value);
            remove => RemoveHandler(ChannelMouseLeaveEvent, value);
        }
        #endregion

        public static readonly DependencyProperty LabelTextContentProperty = DependencyProperty.Register(nameof(LabelTextContent), typeof(string), typeof(ColorChannelUserControl), new PropertyMetadata(""));
        public string LabelTextContent
        {
            get => (string)GetValue(LabelTextContentProperty);
            set => SetValue(LabelTextContentProperty, value);
        }

        public virtual void SetLabelText(string lblText)
        {
            LabelTextContent = lblText;
        }

        protected virtual string GetBtnString(ColorChannelEnum channel) => channel switch
        {
            _ => string.Empty
        };

        protected virtual void UpdateBrushColors(Brush wantedBrush)
        {
            wantedBrush.Opacity = 0.05;
        }

        protected virtual void UpdateColoringStuff(ColorChannelEnum channel)
        {
            UpdateBrushColors(GetColorBrush(channel));
        }

        protected static Brush GetColorBrush(ColorChannelEnum channel, bool isBright = false) => channel switch
        {
            ColorChannelEnum.Green => isBright ? SharedInfo.MainColorGreenBright : SharedInfo.MainColorGreen,
            ColorChannelEnum.Blue => isBright ? SharedInfo.MainColorBlueBright : SharedInfo.MainColorBlue,
            ColorChannelEnum.Alpha => isBright ? SharedInfo.MainColorAlphaBright : SharedInfo.MainColorAlpha,
            _ => isBright ? SharedInfo.MainColorRedBright : SharedInfo.MainColorRed
        };

        public virtual void SetSelected(bool selected)
        {
            UpdateBrushColors(GetColorBrush(ColorChannel, selected));
        }
    }
}
