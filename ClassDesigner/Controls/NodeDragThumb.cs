using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ClassDesigner.Controls
{
    public class NodeDragThumb:Thumb
    {
        public Guid NodeId { get; set; }
        public Point Position { get; set; }
    }
}
