using ImageCombinerChannelExtractor.Components.Classes;
using ImageCombinerChannelExtractor.Components.Classes.UserControlChildClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Interfaces;
using ImageCombinerChannelExtractor.Components.Shared;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ColorChannelInput : ColorChannelInputClass, DragOverInterface
    {
        public ColorChannelInput()
        {
            InitializeComponent();

            DragOverInterface dragHandler = this;
            crdPanel.DragEnter += dragHandler.DraggingIntoWindow;
            crdPanel.DragLeave += dragHandler.DraggingLeaveWindow;

            foreach (ChannelFilteringMode value in Enum.GetValues(typeof(ChannelFilteringMode)))
            {
                cmbboxFiltering.Items.Add(EnumFriendlyNameHelper.GetFriendlyName(value));
            }
            cmbboxFiltering.SelectedIndex = 0;
            UpdateDeleteButtonEnabled(false);

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);

            SetLabelText(StringLinesInfo.NoInputImageTextDefault);
        }

        #region Variables
        public static readonly DependencyProperty SelectedFilteringProperty = DependencyProperty.Register(nameof(SelectedFiltering), typeof(ChannelFilteringMode), typeof(ColorChannelInput), new PropertyMetadata(ChannelFilteringMode.Bicubic));
        public ChannelFilteringMode SelectedFiltering
        {
            get => (ChannelFilteringMode)GetValue(SelectedFilteringProperty);
            set => SetValue(SelectedFilteringProperty, value);
        }
        #endregion

        #region Interaction events
        public static readonly RoutedEvent FilteringChangedEvent = EventManager.RegisterRoutedEvent(nameof(FilteringChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorChannelInput));
        public event RoutedEventHandler FilteringChanged
        {
            add => AddHandler(FilteringChangedEvent, value);
            remove => RemoveHandler(FilteringChangedEvent, value);
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

            if (FileDropHelper.IsFileValid(e))
            {
                e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }

        private void OnButtonDrop(object sender, DragEventArgs e)
        {
            (bool isValid, string? validFile) result = FileDropHelper.IsFileValidAndReturnValidFile(e);
            if (result.isValid)
            {
#pragma warning disable CS8604
                SetLabelText(Path.GetFileName(result.validFile));
#pragma warning restore CS8604
                RaiseEvent(new FileSelectedEventArgs(ChannelClickEvent, this, result.validFile));
            }
        }

        private void OnFilteringChange(object sender, SelectionChangedEventArgs e)
        {
            if (cmbboxFiltering.SelectedIndex >= 0)
            {
                Array modes = Enum.GetValues(typeof(ChannelFilteringMode));
                if (cmbboxFiltering.SelectedIndex < modes.Length)
                {
                    SelectedFiltering = (ChannelFilteringMode)modes.GetValue(cmbboxFiltering.SelectedIndex)!;
                }
            }
            RaiseEvent(new RoutedEventArgs(FilteringChangedEvent, this));
        }
        #endregion

        #region Interfaces
        public void SetDraggingOver(bool draggingOver)
        {
            dropOvrl.Visibility = draggingOver ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion

        public void UpdateDeleteButtonEnabled(bool enabled)
        {
            dltBtn.IsEnabled = enabled;
        }

        #region Overrides
        protected override void UpdateColoringStuff(ColorChannelEnum channel)
        {
            if (btnColorChannelInput is null)
            {
                return;
            }

            btnColorChannelInput.Content = GetBtnString(channel);
            base.UpdateColoringStuff(channel);
        }

        protected override void UpdateBrushColors(Brush wantedBrush)
        {
            base.UpdateBrushColors(wantedBrush);
            crdPanel.Background = wantedBrush;
        }

        protected override string GetBtnString(ColorChannelEnum channel) => channel switch
        {
            ColorChannelEnum.Green => StringLinesInfo.ClrChnBtnGreen,
            ColorChannelEnum.Blue => StringLinesInfo.ClrChnBtnBlue,
            ColorChannelEnum.Alpha => StringLinesInfo.ClrChnBtnAlpha,
            _ => StringLinesInfo.ClrChnBtnRed
        };
        #endregion
    }
}