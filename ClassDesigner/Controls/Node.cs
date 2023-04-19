using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Controls
{
    public class Node
    {
        public Node(Point point)
        {
            Point = point;
            Id = Guid.NewGuid();
        }

        public Node(Point point, Guid id)
        {
            Point = point;
            Id = id;
        }

        public Guid Id { get; }
        public Point Point { get; set; }
    }
}
