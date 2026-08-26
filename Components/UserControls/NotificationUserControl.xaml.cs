using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Shared;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class DoingTaskNotif : UserControl
    {
        public event Action<uint>? CloseRequested;

        private readonly uint index = 0;
        public DoingTaskNotif(NotificationTypeEnum type, string NotificationText, string description, uint index)
        {
            InitializeComponent();
            LayoutTransform = new ScaleTransform { ScaleY = -1 };

            this.index = index;

            TaskInfoBar.Title = NotificationText;
            TaskInfoBar.Message = description;
            TaskInfoBar.Severity = VisualsHelper.GetSeverity(type);

            DependencyPropertyDescriptor.FromProperty(InfoBar.IsOpenProperty, typeof(InfoBar))?.AddValueChanged(TaskInfoBar, OnIsOpenChanged);
        }

        public void SetSecondsLeft(int secondsLeft)
        {
            TaskInfoBar.Message = StringLinesInfo.GetNotificationWillBeRemovedText(secondsLeft);
        }

        private void OnIsOpenChanged(object? sender, EventArgs e)
        {
            if (!TaskInfoBar.IsOpen)
            {
                CloseRequested?.Invoke(index);
            }
        }
    }
}
