using System.Windows;
using System.Windows.Controls;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class LoadingScreen : UserControl
    {
        private readonly Random _random = new Random();
        private const int _minimumProgressRandomize = 1;
        private const int _maximumProgressRandomize = 15;

        public LoadingScreen()
        {
            InitializeComponent();
        }

        public void ShowLoading(string loadingText)
        {
            lblLoadingText.Content = loadingText;
            progressBarLoading.Value = _random.Next(_minimumProgressRandomize, _maximumProgressRandomize);
            this.Visibility = Visibility.Visible;
        }

        public void StopLoading()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void SetProgress(int progress)
        {
            progressBarLoading.Value = progress;
        }
    }
}
