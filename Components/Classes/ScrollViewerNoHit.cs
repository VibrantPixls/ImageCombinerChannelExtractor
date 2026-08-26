using System.Windows.Controls;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes
{
    public class ScrollViewerNoHit : ScrollViewer
    {
        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            HitTestResult? result = base.HitTestCore(hitTestParameters);
            if (result != null && result.VisualHit == this)
            {
                return null;
            }
            return result;
        }
    }
}
