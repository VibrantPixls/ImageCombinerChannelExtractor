using System.Windows;

namespace ImageCombinerChannelExtractor.Components.Interfaces
{
    public interface DragOverInterface
    {
        void SetDraggingOver(bool draggingOver);

        void DraggingIntoWindow(object sender, DragEventArgs e)
        {
            SetDraggingOver(true);
        }

        void DraggingLeaveWindow(object sender, DragEventArgs e)
        {
            SetDraggingOver(false);
        }
    }
}
