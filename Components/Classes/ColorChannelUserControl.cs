using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public partial class ColorChannelUserControl : System.Windows.Controls.UserControl
    {
        #region Variables
        public static readonly DependencyProperty ColorChannelProperty = DependencyProperty.Register(nameof(ColorChannel), typeof(ColorChannelEnum), typeof(ColorChannelUserControl), new PropertyMetadata(ColorChannelEnum.red));
        public ColorChannelEnum ColorChannel
        {
            get => (ColorChannelEnum)GetValue(ColorChannelProperty);
            set => SetValue(ColorChannelProperty, value);
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

        protected virtual void UpdateBrushColors(Brush wantedBrush)
        {

        }

        protected virtual void UpdateColoringStuff(ColorChannelEnum channel)
        {
            UpdateBrushColors(ColorHelper.GetColorBrush(channel));
        }

        public virtual void SetSelected(bool selected)
        {
            UpdateBrushColors(ColorHelper.GetColorBrush(ColorChannel, selected ? ColorBrushTypeEnum.Bright : ColorBrushTypeEnum.Normal));
        }
    }
}
