using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ClassDesigner.Controls
{
    public class DragThumb : Thumb
    {
        public DragThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DesignerItem designerItem = this.DataContext as DesignerItem;
            DesignerCanvas designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
            if (designerItem != null && designer != null && designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                var designerItems = designer.SelectionService.Selection.OfType<DesignerItem>();

                var connections = designer.SelectionService.Selection.OfType<Connection>();

                foreach (DesignerItem item in designerItems)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (DesignerItem item in designerItems)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    Canvas.SetLeft(item, left + deltaHorizontal);
                    Canvas.SetTop(item, top + deltaVertical);

                    foreach (var connection in connections.Where(x => x.Source.ParentDesignerItem == item))
                    {
                        foreach (var node in connection.Nodes)
                        {
                            node.Point = new System.Windows.Point(node.Point.X + deltaHorizontal, node.Point.Y + deltaVertical);
                        }
                        connection.UpdateConnection();
                    }
                }

                designer.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
