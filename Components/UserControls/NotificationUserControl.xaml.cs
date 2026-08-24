using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class DoingTaskNotif : UserControl
    {
        public DoingTaskNotif(NotificationTypeEnum type, string NotificationText)
        {
            InitializeComponent();
            LayoutTransform = new ScaleTransform { ScaleY = -1 };

            TaskInfoBar.Title = NotificationText;
            TaskInfoBar.Severity = VisualsHelper.GetSeverity(type);
        }

        public void SetSecondsLeft(int secondsLeft)
        {
            TaskInfoBar.Message = $"This message wil be deleted in {secondsLeft} seconds";
        }
    }
}
