using ClassDesigner.Helping;
using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using ClassDesigner.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace ClassDesigner.Controls
{
    public partial class DesignerCanvas
    {
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();

        public DesignerCanvas()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringForward, BringForward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringToFront, BringToFront_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendBackward, SendBackward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendToBack, SendToBack_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            this.AllowDrop = true;
            Clipboard.Clear();
        }

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionService.SelectAll();
        }

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();
            PasteFromClipboard();
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }



        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();

            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(this, "WPF Diagram");
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            SaveFile(root);
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    XElement xElement = XElement.Load(openFile.FileName);
                    this.Children.Clear();
                    this.SelectionService.ClearSelection();
                    PasteXml(xElement);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.SelectionService.ClearSelection();
        }

        private void PasteFromClipboard() => PasteXml(LoadSerializedDataFromClipBoard(), 10, 10);

        private void PasteXml(XElement xElement, double offsetX = 0, double offsetY = 0)
        {
            var itemsXML = xElement.Elements("DesignerItems").Elements("DesignerItem");

            this.SelectionService.ClearSelection();

            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();

            foreach (var item in itemsXML)
            {
                var id = Guid.NewGuid();
                mappingOldToNewIDs.Add(Guid.Parse(item.Element("ID").Value), id);
                var di = DeserializeDesignerItem(item, id, offsetX, offsetY);
                SetConnectorDecoratorTemplate(di);
                this.Children.Add(di);
                SelectionService.AddSelection(di);
            }

            var items = this.Children.OfType<DesignerItem>();
            var connections = xElement.Elements("Connections").Elements("Connection");

            IEnumerable<XElement> connectionsXML = xElement.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector, (RelationType)Enum.Parse(typeof(RelationType), connectionXML.Element("RelationType").Value));

                    Nodes n = new Nodes();

                    foreach (var node in connectionXML.Element("Nodes").Elements("Node"))
                    {
                        n.Add(new Node(new Point(Double.Parse(node.Element("X").Value, CultureInfo.InvariantCulture) + offsetX, Double.Parse(node.Element("Y").Value, CultureInfo.InvariantCulture) + offsetY)));
                    }

                    connection.Nodes = n;

                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("ZIndex").Value));
                    this.Children.Add(connection);
                    connection.UpdateConnection();
                    SelectionService.AddSelection(connection);
                }
            }
        }
        

        private static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double offsetX = 0, double offsetY = 0)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Heigth").Value, CultureInfo.InvariantCulture);
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + offsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + offsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("ZIndex").Value));
            Object content = DeserializeClass(itemXML.Element("Content").Element("Class"));
            item.Content = content;
            return item;
        }

        private static ClassViewModel DeserializeClass(XElement itemXML)
        {
            ClassViewModel model = new ClassViewModel();
            model.Header = itemXML.Element("Header").Value;
            model.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), itemXML.Element("Visibility").Value);
            model.IsStatic = bool.Parse(itemXML.Element("IsStatic").Value);
            model.IsAbstract = bool.Parse(itemXML.Element("IsAbstract").Value);
            foreach (var attrXML in itemXML.Element("Attributes").Elements("Attribute"))
            {
                AttributeViewModel attribute = new AttributeViewModel();
                attribute.Name = attrXML.Element("Name").Value;
                attribute.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), attrXML.Element("Visibility").Value);
                attribute.IsStatic = bool.Parse(attrXML.Element("IsStatic").Value);
                attribute.Type = attrXML.Element("Type").Value;
                model.Attributes.Add(attribute);
            }
            foreach (var methodXML in itemXML.Element("Methods").Elements("Method"))
            {
                MethodViewModel method = new MethodViewModel();
                method.Name = methodXML.Element("Name").Value;
                method.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), methodXML.Element("Visibility").Value);
                method.IsStatic = bool.Parse(methodXML.Element("IsStatic").Value);
                method.Type = methodXML.Element("Type").Value;
                foreach (var paramXML in methodXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    method.Parameters.Add(param);
                }
                model.Methods.Add(method);
            }
            foreach (var stereotype in itemXML.Element("Stereotypes").Elements("Stereotype"))
            {
                model.Stereotypes.FirstOrDefault(x => x.Stereotype.ToString() == stereotype.Value).IsSelected = true;
            }
            return model;
        }
        private Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = this.Children.OfType<DesignerItem>().FirstOrDefault(x => x.ID == itemID);

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }
        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }
        private void DeleteCurrentSelection()
        {
            foreach (Connection connection in SelectionService.Selection.OfType<Connection>())
            {
                this.Children.Remove(connection);
            }

            foreach (DesignerItem item in SelectionService.Selection.OfType<DesignerItem>())
            {
                Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;

                List<Connector> connectors = new List<Connector>();
                GetConnectors(cd, connectors);

                foreach (Connector connector in connectors)
                {
                    foreach (Connection con in connector.Connections)
                    {
                        this.Children.Remove(con);
                    }
                }
                this.Children.Remove(item);
            }

            SelectionService.ClearSelection();
            UpdateZIndex();
        }
        private void CopyCurrentSelection()
        {
            List<DesignerItem> selectedDesignerItems = this.SelectionService.Selection.OfType<DesignerItem>().ToList();
            List<Connection> selectedConnections = this.SelectionService.Selection.OfType<Connection>().ToList();

            foreach (var connection in selectedConnections)
            {
                if (!selectedDesignerItems.Contains(connection.Sink.ParentDesignerItem))
                {
                    selectedDesignerItems.Add(connection.Sink.ParentDesignerItem);
                }
                if (!selectedDesignerItems.Contains(connection.Source.ParentDesignerItem))
                {
                    selectedDesignerItems.Add(connection.Source.ParentDesignerItem);
                }
            }

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            XElement connectionsXML = SerializeConnections(selectedConnections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }


        XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                                    designerItems.Select(x =>
                                                    new XElement("DesignerItem",
                                                        new XElement("Left", Canvas.GetLeft(x)),
                                                        new XElement("Top", Canvas.GetTop(x)),
                                                        new XElement("Width", x.Width),
                                                        new XElement("Heigth", x.Height),
                                                        new XElement("ID", x.ID),
                                                        new XElement("ZIndex", Canvas.GetZIndex(x)),
                                                        new XElement("Content",
                                                            SerializeClass(((ClassViewModel)x.Content))
                                                        ))));
            return serializedItems;
        }


        XElement SerializeClass(ClassViewModel model)
        {
            XElement serializedClass = new XElement("Class",
                                                     new XElement("Header", model.Header),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("IsStatic", model.IsStatic),
                                                     new XElement("IsAbstract", model.IsAbstract),
                                                     new XElement("Attributes",
                                                        model.Attributes.Select(a =>
                                                        new XElement("Attribute",
                                                            new XElement("Name", a.Name),
                                                            new XElement("Visibility", a.Visibility),
                                                            new XElement("Type", a.Type),
                                                            new XElement("IsStatic", a.IsStatic)
                                                        ))),
                                                     new XElement("Methods",
                                                        model.Methods.Select(m =>
                                                        new XElement("Method",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Type", m.Type),
                                                            new XElement("IsStatic", m.IsStatic),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Name", p.Type)
                                                                ))
                                                        )))),
                                                     new XElement("Stereotypes",
                                                         model.Stereotypes.Where(s=>s.IsSelected).Select(s =>
                                                         new XElement("Stereotype", s.Stereotype))
                                                     ));
            return serializedClass;
        }

        XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           connections.Select(c => new XElement("Connection",
                                      new XElement("SourceID", c.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", c.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", c.Source.Name),
                                      new XElement("SinkConnectorName", c.Sink.Name),
                                      new XElement("RelationType", c.ConnectionViewModel.RelationType),
                                      new XElement("ZIndex", Canvas.GetZIndex(c)),
                                      new XElement("Nodes",
                                        c.Nodes.Select(n =>
                                        new XElement("Node",
                                            new XElement("X", n.Point.X),
                                            new XElement("Y", n.Point.Y)
                                        )
                                     ))
                                  )));

            return serializedConnections;
        }

        void SaveFile(XElement xElement)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    xElement.Save(saveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateZIndex()
        {
            List<UIElement> ordered = this.Children.OfType<UIElement>().OrderBy(x => Canvas.GetZIndex(x)).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }
    }
}
