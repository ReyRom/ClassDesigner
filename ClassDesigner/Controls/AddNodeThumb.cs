using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ClassDesigner.Controls
{
    class AddNodeThumb : Thumb
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set;}
        public Guid Guid1 { get; set; }
        public Guid Guid2 { get; set; }

        public Point Position { get; set; }
    }
}
