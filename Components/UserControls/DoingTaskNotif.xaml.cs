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

            Icon.Text = WindowsIconHelper.GetWindowsIcon(type);
            MessageText.Text = NotificationText;
        }
    }
}
