using ClassDesigner.Controls;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassDesigner.Helping
{
    public class PathFinder
    {
        public static Point GetMiddlePoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        public static List<Point> GetConnectionLine(ConnectorInfo connectorInfo, Point position, ConnectorOrientation targetOrientation)
        {
            Point point1 = GetPoint(new Rect(new Point(connectorInfo.DesignerItemLeft, connectorInfo.DesignerItemTop), connectorInfo.DesignerItemSize), position); 
            Point point2 = GetMiddlePoint(point1, position);
            return new List<Point> { point1, point2, position };
        }
        public static List<Point> GetConnectionLine(ConnectorInfo connectorInfo1, ConnectorInfo connectorInfo2, Nodes nodes)
        {

            if (nodes.Count==0)
            {
                Point p1 = GetPoint(new Rect(new Point(connectorInfo1.DesignerItemLeft, connectorInfo1.DesignerItemTop), connectorInfo1.DesignerItemSize), 
                    GetMiddlePoint(new Point(connectorInfo2.DesignerItemLeft,connectorInfo2.DesignerItemTop), 
                                   new Point(connectorInfo2.DesignerItemLeft+connectorInfo2.DesignerItemSize.Width, connectorInfo2.DesignerItemTop + connectorInfo2.DesignerItemSize.Height)));
                Point p2 = GetPoint(new Rect(new Point(connectorInfo2.DesignerItemLeft, connectorInfo2.DesignerItemTop), connectorInfo2.DesignerItemSize),
                    GetMiddlePoint(new Point(connectorInfo1.DesignerItemLeft, connectorInfo1.DesignerItemTop), 
                                   new Point(connectorInfo1.DesignerItemLeft + connectorInfo1.DesignerItemSize.Width, connectorInfo1.DesignerItemTop + connectorInfo1.DesignerItemSize.Height)));
                Point mid = GetMiddlePoint(p1,p2);
                nodes.Add(new Node(mid));
            }
            
            Point point1 = GetPoint(new Rect(new Point(connectorInfo1.DesignerItemLeft, connectorInfo1.DesignerItemTop), connectorInfo1.DesignerItemSize), nodes.First().Point);
            Point point2 = GetPoint(new Rect(new Point(connectorInfo2.DesignerItemLeft, connectorInfo2.DesignerItemTop), connectorInfo2.DesignerItemSize), nodes.Last().Point);
            List<Point> result = new List<Point>();
            result.Add(point1);
            result.AddRange(nodes.Points);
            if (point2 == nodes.Last().Point)
            {
                
            }
            else
            {
                result.Add(point2);
            }
            return result;
        }

        public static Point GetPoint(Rect rect, Point point)
        {
            Point rectPoint = new Point();

            if (point.X > rect.Left && point.X < rect.Right)
            {
                rectPoint.X = point.X;
                if (Math.Abs(point.Y - rect.Top) <= Math.Abs(point.Y - rect.Bottom))
                {
                    rectPoint.Y = rect.Top;
                }
                else
                {
                    rectPoint.Y = rect.Bottom;
                }
            }
            else if (point.Y > rect.Top && point.Y < rect.Bottom)
            {
                rectPoint.Y = point.Y;
                if (Math.Abs(point.X - rect.Left) <= Math.Abs(point.X - rect.Right))
                {
                    rectPoint.X = rect.Left;
                }
                else
                {
                    rectPoint.X = rect.Right;
                }
            }
            else if (point.Y <= rect.Top)
            {
                if (point.X >= rect.Right)
                {
                    rectPoint.X = rect.Right;
                    rectPoint.Y = rect.Top;
                }
                else
                {
                    rectPoint.X = rect.Left;
                    rectPoint.Y = rect.Top;
                }
            }
            else if (point.Y >= rect.Bottom)
            {
                if (point.X >= rect.Right)
                {
                    rectPoint.X = rect.Right;
                    rectPoint.Y = rect.Bottom;
                }
                else
                {
                    rectPoint.X = rect.Left;
                    rectPoint.Y = rect.Bottom;
                }
            }
            return rectPoint;
        }

        
    }
}
