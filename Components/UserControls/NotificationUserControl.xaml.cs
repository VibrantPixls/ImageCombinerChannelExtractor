using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
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
        public DoingTaskNotif(NotificationTypeEnum type, string NotificationText, uint index)
        {
            InitializeComponent();
            LayoutTransform = new ScaleTransform { ScaleY = -1 };

            this.index = index;

            TaskInfoBar.Title = NotificationText;
            TaskInfoBar.Severity = VisualsHelper.GetSeverity(type);

            DependencyPropertyDescriptor.FromProperty(InfoBar.IsOpenProperty, typeof(InfoBar))?.AddValueChanged(TaskInfoBar, OnIsOpenChanged);
        }

        public void SetSecondsLeft(int secondsLeft)
        {
            TaskInfoBar.Message = $"This message wil be deleted in {secondsLeft} seconds";
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
