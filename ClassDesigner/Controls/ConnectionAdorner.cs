﻿using ClassDesigner.Helping;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ClassDesigner.Controls
{
    public class ConnectionAdorner : Adorner
    {
        private DesignerCanvas designerCanvas;
        private Canvas adornerCanvas;
        private Connection connection;
        private PathGeometry pathGeometry;
        private Connector fixConnector, dragConnector;
        private Thumb sourceDragThumb, sinkDragThumb;
        private List<Thumb> thumbs = new List<Thumb>();
        private Pen drawingPen;

        private DesignerItem hitDesignerItem;
        private DesignerItem HitDesignerItem
        {
            get { return hitDesignerItem; }
            set
            {
                if (hitDesignerItem != value)
                {
                    if (hitDesignerItem != null)
                        hitDesignerItem.IsDragConnectionOver = false;

                    hitDesignerItem = value;

                    if (hitDesignerItem != null)
                        hitDesignerItem.IsDragConnectionOver = true;
                }
            }
        }

        private Connector hitConnector;
        private Connector HitConnector
        {
            get { return hitConnector; }
            set
            {
                if (hitConnector != value)
                {
                    hitConnector = value;
                }
            }
        }

        private VisualCollection visualChildren;
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }

        public ConnectionAdorner(DesignerCanvas designer, Connection connection)
            : base(designer)
        {
            this.designerCanvas = designer;
            adornerCanvas = new Canvas();
            this.visualChildren = new VisualCollection(this);
            this.visualChildren.Add(adornerCanvas);

            this.connection = connection;
            this.connection.PropertyChanged += new PropertyChangedEventHandler(AnchorPositionChanged);

            InitializeDragThumbs();

            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;

            base.Unloaded += new RoutedEventHandler(ConnectionAdorner_Unloaded);
        }


        void AnchorPositionChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals("AnchorPositionSource") || e.PropertyName.Equals("AnchorPositionSink"))
            {
                InitializeDragThumbs();
                this.InvalidateVisual();
            }
        }

        void thumbDragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {

            this.HitDesignerItem = null;
            this.HitConnector = null;
            this.pathGeometry = null;
            this.connection.StrokeDashArray = null;
            this.connection.UpdateConnection();
            InitializeDragThumbs();
            this.InvalidateVisual();
        }

        Node draggingNode;

        void thumbDragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var t = (Thumb)sender;

            draggingNode = connection.Nodes.GetNodeAtPoint(new Point(Canvas.GetLeft(t), Canvas.GetTop(t)));

            if (draggingNode != null)
            {

                this.pathGeometry = null;
                this.Cursor = Cursors.Cross;
                this.connection.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });


            }

        }

        void thumbDragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point currentPosition = Mouse.GetPosition(this);

            Point point = new Point(currentPosition.X < 0 ? 0 : currentPosition.X,
                                    currentPosition.Y < 0 ? 0 : currentPosition.Y);

            this.pathGeometry = UpdatePathGeometry(point);
            this.InvalidateVisual();
            designerCanvas.InvalidateMeasure();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, drawingPen, this.pathGeometry);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            adornerCanvas.Arrange(new Rect(0, 0, this.designerCanvas.ActualWidth, this.designerCanvas.ActualHeight));
            return finalSize;
        }

        private void ConnectionAdorner_Unloaded(object sender, RoutedEventArgs e)
        {

            foreach (var item in thumbs)
            {
                item.DragDelta -= new DragDeltaEventHandler(thumbDragThumb_DragDelta);
                item.DragStarted -= new DragStartedEventHandler(thumbDragThumb_DragStarted);
                item.DragCompleted -= new DragCompletedEventHandler(thumbDragThumb_DragCompleted);
                item.PreviewMouseRightButtonUp -= new MouseButtonEventHandler(DragThumb_PreviewMouseRightButtonUp);
                item.PreviewMouseDoubleClick -= new MouseButtonEventHandler(AddNodeThumb_PreviewMouseDoubleClick);
            }

        }

        private void InitializeDragThumbs()
        {
            Style dragThumbStyle = connection.FindResource("ConnectionAdornerThumbStyle") as Style;
            Style addThumbStyle = connection.FindResource("ConnectionAdornerAddNodeThumbStyle") as Style;

            thumbs.Clear();
            this.adornerCanvas.Children.Clear();

            foreach (var node in connection.Nodes)
            {
                var dragThumb = new NodeDragThumb();
                Canvas.SetLeft(dragThumb, node.Point.X);
                Canvas.SetTop(dragThumb, node.Point.Y);

                dragThumb.Position = node.Point;
                dragThumb.NodeId = node.Id;

                this.adornerCanvas.Children.Add(dragThumb);
                if (dragThumbStyle != null)
                    dragThumb.Style = dragThumbStyle;
                dragThumb.DragDelta += new DragDeltaEventHandler(thumbDragThumb_DragDelta);
                dragThumb.DragStarted += new DragStartedEventHandler(thumbDragThumb_DragStarted);
                dragThumb.DragCompleted += new DragCompletedEventHandler(thumbDragThumb_DragCompleted);
                dragThumb.PreviewMouseRightButtonUp += new MouseButtonEventHandler(DragThumb_PreviewMouseRightButtonUp);

                thumbs.Add(dragThumb);
            }

            var addNodeThumb1 = new AddNodeThumb();
            addNodeThumb1.Point1 = connection.AnchorPositionSource;
            addNodeThumb1.Point2 = connection.Nodes[0].Point;
            addNodeThumb1.Guid2 = connection.Nodes[0].Id;
            var pt1 = PathFinder.GetMiddlePoint(addNodeThumb1.Point1, addNodeThumb1.Point2);
            Canvas.SetLeft(addNodeThumb1, pt1.X);
            Canvas.SetTop(addNodeThumb1, pt1.Y);
            addNodeThumb1.Position = pt1;
            this.adornerCanvas.Children.Add(addNodeThumb1);
            if (dragThumbStyle != null)
                addNodeThumb1.Style = addThumbStyle;
            addNodeThumb1.PreviewMouseDoubleClick += new MouseButtonEventHandler(AddNodeThumb_PreviewMouseDoubleClick);

            thumbs.Add(addNodeThumb1);

            for (int i = 1; i < connection.Nodes.Count; i++)
            {
                var addNodeThumb = new AddNodeThumb();
                addNodeThumb.Point1 = connection.Nodes[i - 1].Point;
                addNodeThumb.Guid1 = connection.Nodes[i - 1].Id;
                addNodeThumb.Point2 = connection.Nodes[i].Point;
                addNodeThumb.Guid2 = connection.Nodes[i].Id;
                var pt = PathFinder.GetMiddlePoint(addNodeThumb.Point1, addNodeThumb.Point2);
                Canvas.SetLeft(addNodeThumb, pt.X);
                Canvas.SetTop(addNodeThumb, pt.Y);
                addNodeThumb.Position = pt;
                this.adornerCanvas.Children.Add(addNodeThumb);
                if (dragThumbStyle != null)
                    addNodeThumb.Style = addThumbStyle;
                addNodeThumb.PreviewMouseDoubleClick += new MouseButtonEventHandler(AddNodeThumb_PreviewMouseDoubleClick);

                thumbs.Add(addNodeThumb);
            }

            var addNodeThumb2 = new AddNodeThumb();
            addNodeThumb2.Point1 = connection.Nodes.Last().Point;
            addNodeThumb2.Guid1 = connection.Nodes.Last().Id;
            addNodeThumb2.Point2 = connection.AnchorPositionSink;
            var pt2 = PathFinder.GetMiddlePoint(addNodeThumb2.Point1, addNodeThumb2.Point2);
            Canvas.SetLeft(addNodeThumb2, pt2.X);
            Canvas.SetTop(addNodeThumb2, pt2.Y);
            addNodeThumb2.Position = pt2;
            this.adornerCanvas.Children.Add(addNodeThumb2);
            if (dragThumbStyle != null)
                addNodeThumb2.Style = addThumbStyle;
            addNodeThumb2.PreviewMouseDoubleClick += new MouseButtonEventHandler(AddNodeThumb_PreviewMouseDoubleClick);

            thumbs.Add(addNodeThumb2);

        }

        private void AddNodeThumb_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var t = (AddNodeThumb)sender;
            if (t.Guid1 == null)
            {
                connection.Nodes.InsertNode(t.Guid2, new Node(t.Position));
            }
            else
            {
                connection.Nodes.InsertNode(t.Guid1, new Node(t.Position), false);
            }

            this.connection.UpdateConnection();
            InitializeDragThumbs();
            this.InvalidateVisual();
        }

        private void DragThumb_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (connection.Nodes.Count > 1)
            {
                var t = (NodeDragThumb)sender;
                var node = connection.Nodes.GetNodeById(t.NodeId);
                if (node != null)
                {
                    connection.Nodes.Remove(node);
                }
                connection.UpdateConnection();
                InitializeDragThumbs();
                this.InvalidateVisual();
            }
        }

        private PathGeometry UpdatePathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();

            connection.Nodes.UpdateNode(draggingNode.Id, position);

            List<Point> linePoints = PathFinder.GetConnectionLine(connection.Source.GetInfo(), connection.Sink.GetInfo(), connection.Nodes);

            if (linePoints.Count > 0)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = linePoints[0];
                linePoints.Remove(linePoints[0]);
                figure.Segments.Add(new PolyLineSegment(linePoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        private void HitTesting(Point hitPoint)
        {
            bool hitConnectorFlag = false;

            DependencyObject hitObject = designerCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != fixConnector.ParentDesignerItem &&
                   hitObject.GetType() != typeof(DesignerCanvas))
            {
                if (hitObject is Connector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                }

                if (hitObject is DesignerItem)
                {
                    HitDesignerItem = hitObject as DesignerItem;
                    if (!hitConnectorFlag)
                        HitConnector = null;
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitDesignerItem = null;
        }

    }
}
