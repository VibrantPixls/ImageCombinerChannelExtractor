using System.Windows.Controls;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes.NoHittable
{
    public class StackPanelNoHit : StackPanel
    {
        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            HitTestResult? result = base.HitTestCore(hitTestParameters);
            return result?.VisualHit == this ? null : result;
        }
    }
}
