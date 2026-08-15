using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class CheckerboardPictureBox : UserControl
    {
        public CheckerboardPictureBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CheckerboardPictureBox), new PropertyMetadata(null));
        public ImageSource? ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
    }
}
