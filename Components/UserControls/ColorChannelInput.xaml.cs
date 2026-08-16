using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ColorChannelInput : UserControl
    {
        public ColorChannelInput()
        {
            InitializeComponent();

            foreach (ChannelFilteringMode value in Enum.GetValues(typeof(ChannelFilteringMode)))
            {
                cmbboxFiltering.Items.Add(EnumFriendlyNameHelper.GetFriendlyName(value));
            }
            cmbboxFiltering.SelectedIndex = 0;
        }

        public static readonly DependencyProperty ColorChannelProperty = DependencyProperty.Register(nameof(ColorChannel), typeof(ColorChannelEnum), typeof(ColorChannelInput), new PropertyMetadata(ColorChannelEnum.Red));
        public ColorChannelEnum ColorChannel
        {
            get => (ColorChannelEnum)GetValue(ColorChannelProperty);
            set => SetValue(ColorChannelProperty, value);
        }

        public static readonly DependencyProperty ChannelColorProperty = DependencyProperty.Register(nameof(ChannelColor), typeof(Brush), typeof(ColorChannelInput), new PropertyMetadata(Brushes.White));
        public Brush ChannelColor
        {
            get => (Brush)GetValue(ChannelColorProperty);
            set => SetValue(ChannelColorProperty, value);
        }

        public static readonly DependencyProperty IsChannelFromCombinedProperty = DependencyProperty.Register(nameof(IsChannelFromCombined), typeof(bool), typeof(ColorChannelInput), new PropertyMetadata(true));
        public bool IsChannelFromCombined
        {
            get => (bool)GetValue(IsChannelFromCombinedProperty);
            set => SetValue(IsChannelFromCombinedProperty, value);
        }

        public static readonly DependencyProperty SelectedFilteringProperty = DependencyProperty.Register(nameof(SelectedFiltering), typeof(ChannelFilteringMode), typeof(ColorChannelInput), new PropertyMetadata(ChannelFilteringMode.Bicubic));
        public ChannelFilteringMode SelectedFiltering
        {
            get => (ChannelFilteringMode)GetValue(SelectedFilteringProperty);
            set => SetValue(SelectedFilteringProperty, value);
        }

        #region Interaction events
        public static readonly RoutedEvent FilteringChangedEvent = EventManager.RegisterRoutedEvent(nameof(FilteringChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelInput));
        public event RoutedEventHandler FilteringChanged
        {
            add => AddHandler(FilteringChangedEvent, value);
            remove => RemoveHandler(FilteringChangedEvent, value);
        }

        public static readonly RoutedEvent ChannelClickEvent = EventManager.RegisterRoutedEvent(nameof(ChannelClick), RoutingStrategy.Bubble, typeof(EventHandler<FileSelectedEventArgs>), typeof(ColorChannelInput));
        public event EventHandler<FileSelectedEventArgs> ChannelClick
        {
            add => AddHandler(ChannelClickEvent, value);
            remove => RemoveHandler(ChannelClickEvent, value);
        }

        public static readonly RoutedEvent ChannelClickRemoveEvent = EventManager.RegisterRoutedEvent(nameof(ChannelClickRemove), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelInput));
        public event RoutedEventHandler ChannelClickRemove
        {
            add => AddHandler(ChannelClickRemoveEvent, value);
            remove => RemoveHandler(ChannelClickRemoveEvent, value);
        }

        public static readonly RoutedEvent ChannelMouseEnterEvent = EventManager.RegisterRoutedEvent(nameof(ChannelMouseEnter), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelInput));
        public event RoutedEventHandler ChannelMouseEnter
        {
            add => AddHandler(ChannelMouseEnterEvent, value);
            remove => RemoveHandler(ChannelMouseEnterEvent, value);
        }

        public static readonly RoutedEvent ChannelMouseLeaveEvent = EventManager.RegisterRoutedEvent(nameof(ChannelMouseLeave), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelInput));
        public event RoutedEventHandler ChannelMouseLeave
        {
            add => AddHandler(ChannelMouseLeaveEvent, value);
            remove => RemoveHandler(ChannelMouseLeaveEvent, value);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new FileSelectedEventArgs(ChannelClickEvent, this, null));
        }

        private void OnButtonClickRemove(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelClickRemoveEvent, this));
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseEnterEvent, this));
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChannelMouseLeaveEvent, this));
        }

        private void OnButtonDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && files.Length > 0 && IsValidImageFile(files[0]))
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
                string? validFile = files?.FirstOrDefault(IsValidImageFile);
                if (!string.IsNullOrEmpty(validFile))
                {
                    SetLabelText(Path.GetFileName(validFile));
                    RaiseEvent(new FileSelectedEventArgs(ChannelClickEvent, this, validFile));
                }
            }
        }

        private void OnFilteringChange(object sender, SelectionChangedEventArgs e)
        {
            if (cmbboxFiltering.SelectedIndex >= 0)
            {
                var modes = Enum.GetValues(typeof(ChannelFilteringMode));
                if (cmbboxFiltering.SelectedIndex < modes.Length)
                {
                    SelectedFiltering = (ChannelFilteringMode)modes.GetValue(cmbboxFiltering.SelectedIndex)!;
                }
            }
            RaiseEvent(new RoutedEventArgs(FilteringChangedEvent, this));
        }
        #endregion

        public void SetLabelText(string lblText)
        {
            lblSelectedFile.Text = lblText;
        }

        private static bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string extension = Path.GetExtension(filePath);
            return SharedInfo.AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}