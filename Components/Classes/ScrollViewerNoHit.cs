using System.Windows.Controls;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class ScrollViewerNoHit : ScrollViewer
    {
        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            HitTestResult? result = base.HitTestCore(hitTestParameters);
            return result?.VisualHit == this ? null : result;
        }
    }
}
