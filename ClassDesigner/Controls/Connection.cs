using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Runtime.CompilerServices;

namespace ClassDesigner.Controls
{
    public class Connection : Control, ISelectable, INotifyPropertyChanged
    {
        private Adorner connectionAdorner;

        public ConnectionViewModel ConnectionViewModel
        {
            get => connectionViewModel; set
            {
                connectionViewModel = value;
            }
        }

        #region Properties

        public Guid ID { get; set; }

        // source connector
        private Connector source;
        public Connector Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    if (source != null)
                    {
                        source.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        source.Connections.Remove(this);
                    }

                    source = value;

                    if (source != null)
                    {
                        source.Connections.Add(this);
                        source.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }

                    UpdatePathGeometry();
                }
            }
        }

        // sink connector
        private Connector sink;
        public Connector Sink
        {
            get { return sink; }
            set
            {
                if (sink != value)
                {
                    if (sink != null)
                    {
                        sink.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        sink.Connections.Remove(this);
                    }

                    sink = value;

                    if (sink != null)
                    {
                        sink.Connections.Add(this);
                        sink.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }
                    UpdatePathGeometry();
                }
            }
        }

        Nodes nodes = new Nodes();
        public Nodes Nodes
        {
            get => nodes;
            set
            {
                if (nodes != value)
                {
                    nodes = value;
                    OnPropertyChanged("Nodes");
                }
            }
        }

        // connection path geometry
        private PathGeometry pathGeometry;
        public PathGeometry PathGeometry
        {
            get { return pathGeometry; }
            set
            {
                if (pathGeometry != value)
                {
                    pathGeometry = value;
                    UpdateAnchorPosition();
                    OnPropertyChanged("PathGeometry");
                }
            }
        }

        // between source connector position and the beginning 
        // of the path geometry we leave some space for visual reasons; 
        // so the anchor position source really marks the beginning 
        // of the path geometry on the source side
        private Point anchorPositionSource;
        public Point AnchorPositionSource
        {
            get { return anchorPositionSource; }
            set
            {
                if (anchorPositionSource != value)
                {
                    anchorPositionSource = value;
                    OnPropertyChanged("AnchorPositionSource");
                }
            }
        }

        // slope of the path at the anchor position
        // needed for the rotation angle of the arrow
        private double anchorAngleSource = 0;
        public double AnchorAngleSource
        {
            get { return anchorAngleSource; }
            set
            {
                if (anchorAngleSource != value)
                {
                    anchorAngleSource = value;
                    OnPropertyChanged("AnchorAngleSource");
                }
            }
        }

        // analogue to source side
        private Point anchorPositionSink;
        public Point AnchorPositionSink
        {
            get { return anchorPositionSink; }
            set
            {
                if (anchorPositionSink != value)
                {
                    anchorPositionSink = value;
                    OnPropertyChanged("AnchorPositionSink");
                }
            }
        }
        // analogue to source side
        private double anchorAngleSink = 0;
        public double AnchorAngleSink
        {
            get { return anchorAngleSink; }
            set
            {
                if (anchorAngleSink != value)
                {
                    anchorAngleSink = value;
                    OnPropertyChanged("AnchorAngleSink");
                }
            }
        }

        private ArrowSymbol sourceArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SourceArrowSymbol
        {
            get { return sourceArrowSymbol; }
            set
            {
                if (sourceArrowSymbol != value)
                {
                    sourceArrowSymbol = value;
                    UpdateConnection();
                    OnPropertyChanged("SourceArrowSymbol");
                }
            }
        }

        public ArrowSymbol sinkArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SinkArrowSymbol
        {
            get { return sinkArrowSymbol; }
            set
            {
                if (sinkArrowSymbol != value)
                {
                    sinkArrowSymbol = value;
                    UpdateConnection();
                    OnPropertyChanged("SinkArrowSymbol");
                }
            }
        }

        // specifies a point at half path length
        private Point labelPosition;
        public Point LabelPosition
        {
            get { return labelPosition; }
            set
            {
                if (labelPosition != value)
                {
                    labelPosition = value;
                    OnPropertyChanged("LabelPosition");
                }
            }
        }

        // pattern of dashes and gaps that is used to outline the connection path
        private DoubleCollection strokeDashArray;
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return strokeDashArray;
            }
            set
            {
                if (strokeDashArray != value)
                {
                    strokeDashArray = value;
                    OnPropertyChanged("StrokeDashArray");
                }
            }
        }
        // if connected, the ConnectionAdorner becomes visible
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (isSelected)
                        ShowAdorner();
                    else
                        HideAdorner();
                }
            }
        }



        #endregion

        public Connection(Connector source, Connector sink)
        {
            this.ID = Guid.NewGuid();
            this.Source = source;
            this.Sink = sink;
            var relationType = SettingsService.Instance.RelationType;

            ConnectionViewModel = new ConnectionViewModel(
                (IEntry)source.ParentDesignerItem.Content,
                (IEntry)sink.ParentDesignerItem.Content,
                relationType);

            switch (connectionViewModel.RelationType)
            {
                case RelationType.Dependency:
                    SinkArrowSymbol = ArrowSymbol.Arrow;
                    break;
                case RelationType.Association:
                    SinkArrowSymbol = ArrowSymbol.None;
                    break;
                case RelationType.Aggregation:
                    SinkArrowSymbol = ArrowSymbol.Diamond;
                    break;
                case RelationType.Composition:
                    SinkArrowSymbol = ArrowSymbol.Diamond;
                    break;
                case RelationType.Generalization:
                    SinkArrowSymbol = ArrowSymbol.ClosedArrow;
                    break;
                case RelationType.Realization:
                    SinkArrowSymbol = ArrowSymbol.ClosedArrow;
                    break;
            }

            base.Unloaded += new RoutedEventHandler(Connection_Unloaded);
        }


        public Connection(Connector source, Connector sink, RelationType relationType)
        {
            this.ID = Guid.NewGuid();
            this.Source = source;
            this.Sink = sink;

            ConnectionViewModel = new ConnectionViewModel(
                (IEntry)source.ParentDesignerItem.Content,
                (IEntry)sink.ParentDesignerItem.Content,
                relationType);

            switch (connectionViewModel.RelationType)
            {
                case RelationType.Dependency:
                    SinkArrowSymbol = ArrowSymbol.Arrow;
                    break;
                case RelationType.Association:
                    SinkArrowSymbol = ArrowSymbol.None;
                    break;
                case RelationType.Aggregation:
                    SinkArrowSymbol = ArrowSymbol.Diamond;
                    break;
                case RelationType.Composition:
                    SinkArrowSymbol = ArrowSymbol.Diamond;
                    break;
                case RelationType.Generalization:
                    SinkArrowSymbol = ArrowSymbol.ClosedArrow;
                    break;
                case RelationType.Realization:
                    SinkArrowSymbol = ArrowSymbol.ClosedArrow;
                    break;
            }
            base.Unloaded += new RoutedEventHandler(Connection_Unloaded);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // usual selection business
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddSelection(this);
                        //if (!this.Sink.ParentDesignerItem.IsSelected) 
                        //    designer.SelectionService.AddSelection(this.Sink.ParentDesignerItem);

                        //if (!this.Source.ParentDesignerItem.IsSelected) 
                        //    designer.SelectionService.AddSelection(this.Source.ParentDesignerItem);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);

                    //if (!this.Sink.ParentDesignerItem.IsSelected)
                    //    designer.SelectionService.AddSelection(this.Sink.ParentDesignerItem);

                    //if (!this.Source.ParentDesignerItem.IsSelected)
                    //    designer.SelectionService.AddSelection(this.Source.ParentDesignerItem);
                }

                //Focus();

                PropertiesService.Instance.Selected = this.ConnectionViewModel;
            }
            
            e.Handled = true;
        }

        void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            // whenever the 'Position' property of the source or sink Connector 
            // changes we must update the connection path geometry
            if (e.PropertyName.Equals("Position"))
            {
                UpdatePathGeometry();
            }
        }

        public void UpdateConnection() => UpdatePathGeometry();
        private void UpdatePathGeometry()
        {
            if (Source != null && Sink != null)
            {
                PathGeometry geometry = new PathGeometry();
                List<Point> linePoints = PathFinder.GetConnectionLine(Source.GetInfo(), Sink.GetInfo(), Nodes);

                AnchorPositionSink = linePoints.Last();
                AnchorPositionSource = linePoints.First();



                if (linePoints.Count > 0)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = linePoints[0];
                    linePoints.Remove(linePoints[0]);
                    figure.Segments.Add(new PolyLineSegment(linePoints, true));
                    figure.IsFilled = false;




                    geometry.Figures.Add(figure);
                    this.PathGeometry = geometry;

                    PathGeometry headerSinkGeometry = new PathGeometry();

                    PathFigure headerSinkFigure;

                    switch (SinkArrowSymbol)
                    {
                        case ArrowSymbol.None:
                            headerSinkFigure = new PathFigure();
                            break;
                        case ArrowSymbol.Arrow:
                            headerSinkFigure = CalculateArrow(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSource, AnchorPositionSink, false);
                            break;
                        case ArrowSymbol.ClosedArrow:
                            headerSinkFigure = CalculateArrow(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSource, AnchorPositionSink, true);
                            break;
                        case ArrowSymbol.Diamond:
                            headerSinkFigure = CalculateDiamond(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSource, AnchorPositionSink);
                            break;
                        default:
                            headerSinkFigure = new PathFigure();
                            break;
                    }

                    headerSinkGeometry.Figures.Add(headerSinkFigure);
                    this.HeaderSinkPathGeometry = headerSinkGeometry;


                    PathGeometry headerSourceGeometry = new PathGeometry();

                    PathFigure headerSourceFigure;

                    switch (SinkArrowSymbol)
                    {
                        case ArrowSymbol.None:
                            headerSourceFigure = new PathFigure();
                            break;
                        case ArrowSymbol.Arrow:
                            headerSourceFigure = CalculateArrow(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSink, AnchorPositionSource, false);
                            break;
                        case ArrowSymbol.ClosedArrow:
                            headerSourceFigure = CalculateArrow(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSink, AnchorPositionSource, true);
                            break;
                        case ArrowSymbol.Diamond:
                            headerSourceFigure = CalculateDiamond(linePoints.Count > 1 ? linePoints[^2] : AnchorPositionSink, AnchorPositionSource);
                            break;
                        default:
                            headerSourceFigure = new PathFigure();
                            break;
                    }

                    headerSourceGeometry.Figures.Add(headerSourceFigure);
                    this.HeaderSourcePathGeometry = headerSourceGeometry;


                }
            }
        }


        private PathGeometry headerSinkPathGeometry;
        public PathGeometry HeaderSinkPathGeometry
        {
            get { return headerSinkPathGeometry; }
            set
            {
                if (headerSinkPathGeometry != value)
                {
                    headerSinkPathGeometry = value;
                    UpdateAnchorPosition();
                    OnPropertyChanged("HeaderSinkPathGeometry");
                }
            }
        }

        private PathGeometry headerSourcePathGeometry;
        private ConnectionViewModel connectionViewModel;

        public PathGeometry HeaderSourcePathGeometry
        {
            get { return headerSourcePathGeometry; }
            set
            {
                if (headerSourcePathGeometry != value)
                {
                    headerSourcePathGeometry = value;
                    UpdateAnchorPosition();
                    OnPropertyChanged("HeaderSourcePathGeometry");
                }
            }
        }

        PathFigure CalculateArrow(Point pt1, Point pt2, bool isClosed)
        {
            PathFigure figure = new PathFigure();

            List<Point> points = new List<Point>();

            Matrix matx = new Matrix();
            Vector vect = pt1 - pt2;
            vect.Normalize();
            vect *= 15;

            matx.Rotate(45.0 / 2);
            points.Add(pt2 + vect * matx);

            points.Add(pt2);

            matx.Rotate(-45.0);
            points.Add(pt2 + vect * matx);

            figure.StartPoint = points[0];
            points.Remove(points[0]);
            figure.Segments.Add(new PolyLineSegment(points, true));

            figure.IsClosed = isClosed;
            figure.IsFilled = isClosed;

            return figure;
        }

        PathFigure CalculateDiamond(Point pt1, Point pt2)
        {
            PathFigure figure = new PathFigure();

            List<Point> points = new List<Point>();

            Matrix matx = new Matrix();
            Vector vect = pt1 - pt2;
            vect.Normalize();
            vect *= 15;

            matx.Rotate(45.0 / 2);
            Point p1 = pt2 + vect * matx;
            points.Add(p1);

            points.Add(pt2);

            matx.Rotate(-45.0);
            Point p2 = pt2 + vect * matx;
            points.Add(pt2 + vect * matx);

            matx.Rotate(45.0);

            points.Add(p2 + vect * matx);

            figure.StartPoint = points[0];
            points.Remove(points[0]);
            figure.Segments.Add(new PolyLineSegment(points, true));

            figure.IsClosed = true;
            figure.IsFilled = true;

            return figure;
        }



        private void UpdateAnchorPosition()
        {
            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;
            Point pathMidPoint, pathTangentAtMidPoint;

            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
            // on PathGeometry at the specified fraction of its length
            this.PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
            this.PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
            this.PathGeometry.GetPointAtFractionLength(0.5, out pathMidPoint, out pathTangentAtMidPoint);

            // get angle from tangent vector
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            //// add some margin on source and sink side for visual reasons only
            //pathStartPoint.Offset(pathTangentAtStartPoint.X * 5, pathTangentAtStartPoint.Y * 5);
            //pathEndPoint.Offset(-pathTangentAtEndPoint.X * 5, -pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
            this.LabelPosition = pathMidPoint;
        }

        private void ShowAdorner()
        {
            // the ConnectionAdorner is created once for each Connection
            if (this.connectionAdorner == null)
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    this.connectionAdorner = new ConnectionAdorner(designer, this);
                    adornerLayer.Add(this.connectionAdorner);
                }
            }
            this.connectionAdorner.Visibility = Visibility.Visible;
        }

        internal void HideAdorner()
        {
            if (this.connectionAdorner != null)
                this.connectionAdorner.Visibility = Visibility.Collapsed;
        }

        void Connection_Unloaded(object sender, RoutedEventArgs e)
        {
            // do some housekeeping when Connection is unloaded

            // remove event handler
            this.Source = null;
            this.Sink = null;

            // remove adorner
            if (this.connectionAdorner != null)
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.connectionAdorner);
                    this.connectionAdorner = null;
                }
            }
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    public enum ArrowSymbol
    {
        None,
        Arrow,
        ClosedArrow,
        Diamond
    }
}
