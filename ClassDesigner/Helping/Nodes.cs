using ClassDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ClassDesigner.Helping
{
    public class Nodes : List<Node>
    {
        public List<Point> Points => this.Select(x => x.Point).ToList();

        public void UpdateNode(Guid guid, Point point)
        {
            this.ForEach(x =>
            {
                if (x.Id == guid)
                {
                    x.Point = point;
                }
            });
        }

        public Node? GetNodeAtPoint(Point point)
        {
            return this.FirstOrDefault(x => x.Point == point);
        }

        public Node? GetNodeById(Guid id)   
        {
            return this.FirstOrDefault(x => x.Id == id);
        }

        public void InsertNode(Guid guid, Node node, bool before = true)
        {
            if (before)
            {
                this.Insert(this.FindIndex(x => x.Id == guid) - 1, node);
            }
            else
            {
                this.Insert(this.FindIndex(x => x.Id == guid) + 1, node);
            }
        }
    }
}
