using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageCombinerChannelExtractor.Components.Classes.NoHittable
{
    public class ScrollViewerNoHit : ScrollViewer
    {
        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            HitTestResult? result = base.HitTestCore(hitTestParameters);
            return result?.VisualHit == this ? null : result;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            e.Handled = true; // prevent scrolling
        }
    }
}
