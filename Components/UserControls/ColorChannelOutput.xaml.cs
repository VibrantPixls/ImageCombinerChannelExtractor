using ImageCombinerChannelExtractor.Components.Classes.UserControlChildClasses;
using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.UserControls
{
    public partial class ColorChannelOutput : ColorChannelOutputClass
    {
        public ColorChannelOutput()
        {
            InitializeComponent();

            Loaded += (s, e) => UpdateColoringStuff(ColorChannel);
            SetLabelText(StringLinesInfo.NoInputImageTextDefault);
        }

        protected override void UpdateColoringStuff(ColorChannelEnum channel)
        {
            if (crdPanel is null)
            {
                return;
            }

            crdPanel.Content = GetBtnString(channel);
            base.UpdateColoringStuff(channel);
        }

        protected override void UpdateBrushColors(Brush wantedBrush)
        {
            base.UpdateBrushColors(wantedBrush);
            crdPanel.Background = wantedBrush;
        }

        protected override string GetBtnString(ColorChannelEnum channel) => channel switch
        {
            _ => StringLinesInfo.CrtExtractBtnNoInputs
        };
    }
}
