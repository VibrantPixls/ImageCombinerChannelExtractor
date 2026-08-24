using ImageCombinerChannelExtractor.Components.Helpers;
using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Interfaces
{
    public interface DragOverInterface
    {
        void SetDraggingOver(bool draggingOver);

        void DraggingIntoWindow(object sender, DragEventArgs e)
        {
            SetDraggingOver(FileDropHelper.IsFileValid(e));
        }

        void DraggingLeaveWindow(object sender, DragEventArgs e)
        {
            SetDraggingOver(false);
        }
    }
}
