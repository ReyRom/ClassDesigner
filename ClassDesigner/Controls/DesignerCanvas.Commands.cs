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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        public static RoutedCommand GenerateCode = new RoutedCommand();
        public static RoutedCommand FocusProperty = new RoutedCommand();
        public static RoutedCommand OpenSettings = new RoutedCommand();

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
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.FocusProperty, FocusProperty_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.OpenSettings, OpenSettings_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.GenerateCode, GenerateCode_Executed, GenerateCode_Enabled));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            this.AllowDrop = true;

            Clipboard.Clear();
        }

        private void OpenSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AppSettingsWindow appSettingsWindow = new AppSettingsWindow();
            appSettingsWindow.ShowDialog();
        }

        private void FocusProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PropertiesService.Instance.Selected = e.Parameter;
        }

        private void GenerateCode_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !App.CodeGenerationWindow.IsLoaded;
        }

        private void GenerateCode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.NewCodeGenerationWindow.Show();

        }

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionService.SelectAll();
        }


        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
            GC.Collect();
            DataService.Instance.UpdateEntries(this.Children.OfType<DesignerItem>().Select(x => x.Content).OfType<IEntry>());
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SelectionService.ClearSelection();
                PasteFromClipboard();
                CopyCurrentSelection();
            }
            catch (Exception ex)
            {
                MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok);
            }
            
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
            openFile.Filter = "Class Designer Files|*.classdsgn|Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == true)
            {
                OpenFile(openFile.FileName);
            }
        }

        public void OpenFile(string filename)
        {
            try
            {
                XElement xElement = XElement.Load(filename);
                this.Children.Clear();
                this.SelectionService.ClearSelection();
                PasteXml(xElement);
                SelectionService.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok);
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
            try
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

                        switch ((RelationType)Enum.Parse(typeof(RelationType), connectionXML.Element("RelationType").Value))
                        {
                            case RelationType.Association:
                                break;
                            case RelationType.Aggregation:
                                (connection.ConnectionViewModel.ConnectionData as AggregationDataViewModel).AggregatedAttribute
                                    = (connection.ConnectionViewModel.TargetEntry as IHaveAttributes).Attributes
                                    .FirstOrDefault(x => x.ToString() == connectionXML.Element("ConnectionData").Element("AggregatedAttribute").Value);
                                (connection.ConnectionViewModel.ConnectionData as AggregationDataViewModel).AggregatedAction
                                    = (connection.ConnectionViewModel.TargetEntry as IHaveActions).Actions
                                    .FirstOrDefault(x => x.ToString() == connectionXML.Element("ConnectionData").Element("AggregatedMethod").Value);
                                break;
                            case RelationType.Composition:
                                (connection.ConnectionViewModel.ConnectionData as CompositionDataViewModel).ComposedAttribute
                                    = (connection.ConnectionViewModel.TargetEntry as IHaveAttributes).Attributes
                                    .FirstOrDefault(x => x.ToString() == connectionXML.Element("ConnectionData").Element("ComposedAttribute").Value);
                                break;
                            case RelationType.Generalization:
                                break;
                            case RelationType.Realization:
                                break;
                            case RelationType.Dependency:
                                (connection.ConnectionViewModel.ConnectionData as DependencyDataViewModel).DependencedAction
                                    = (connection.ConnectionViewModel.SourceEntry as IHaveActions).Actions
                                    .FirstOrDefault(x => x.ToString() == connectionXML.Element("ConnectionData").Element("DependencedMethod").Value);
                                break;
                            default:
                                break;
                        }
                        this.Children.Add(connection);
                        connection.UpdateConnection();
                        SelectionService.AddSelection(connection);
                    }
                }
                UpdateZIndex();
                DataService.Instance.UpdateEntries(this.Children.OfType<DesignerItem>().Select(x => x.Content).OfType<IEntry>());
            }
            catch (Exception)
            {
                MessageBox.MessageBox.Show("Ошибка", "Не удалось вставить объект", MessageBox.MessageBoxButtons.Ok);
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
            item.Content = DeserializeContent(itemXML);
            return item;
        }

        private static Object DeserializeContent(XElement itemXML)
        {
            Object content = null;
            if (itemXML.Element("Content").Element("Class") is not null)
            {
                content = DeserializeClass(itemXML.Element("Content").Element("Class"));
            }
            else if (itemXML.Element("Content").Element("Interface") is not null)
            {
                content = DeserializeInterface(itemXML.Element("Content").Element("Interface"));
            }
            else if (itemXML.Element("Content").Element("Struct") is not null)
            {
                content = DeserializeStruct(itemXML.Element("Content").Element("Struct"));
            }
            else if (itemXML.Element("Content").Element("Enum") is not null)
            {
                content = DeserializeEnum(itemXML.Element("Content").Element("Enum"));
            }
            return content;
        }

        private static ClassViewModel DeserializeClass(XElement itemXML)
        {
            ClassViewModel model = new ClassViewModel();
            model.Name = itemXML.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                DataService.Instance.ProvideName(model);
            }
            model.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), itemXML.Element("Visibility").Value);
            model.IsStatic = bool.Parse(itemXML.Element("IsStatic").Value);
            model.IsAbstract = bool.Parse(itemXML.Element("IsAbstract").Value);
            foreach (var attrXML in itemXML.Element("Attributes").Elements("Attribute"))
            {
                FieldViewModel attribute = new FieldViewModel(model);
                attribute.Name = attrXML.Element("Name").Value;
                attribute.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), attrXML.Element("Visibility").Value);
                attribute.IsStatic = bool.Parse(attrXML.Element("IsStatic").Value);
                attribute.Type = attrXML.Element("Type").Value;
                attribute.DefaultValue = attrXML.Element("DefaultValue").Value;
                model.Attributes.Add(attribute);
            }
            foreach (var propXML in itemXML.Element("Attributes").Elements("Property"))
            {
                PropertyViewModel property = new PropertyViewModel(model);
                property.Name = propXML.Element("Name").Value;
                property.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), propXML.Element("Visibility").Value);
                property.IsStatic = bool.Parse(propXML.Element("IsStatic").Value);
                property.IsAbstract = bool.Parse(propXML.Element("IsAbstract").Value);
                property.IsGet = bool.Parse(propXML.Element("IsGet").Value);
                property.IsSet = bool.Parse(propXML.Element("IsSet").Value);
                property.Type = propXML.Element("Type").Value;
                property.DefaultValue = propXML.Element("DefaultValue").Value;
                model.Attributes.Add(property);
            }
            foreach (var methodXML in itemXML.Element("Actions").Elements("Method"))
            {
                MethodViewModel method = new MethodViewModel(model);
                method.Name = methodXML.Element("Name").Value;
                method.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), methodXML.Element("Visibility").Value);
                method.IsStatic = bool.Parse(methodXML.Element("IsStatic").Value);
                method.IsAbstract = bool.Parse(methodXML.Element("IsAbstract").Value);
                method.Type = methodXML.Element("Type").Value;
                foreach (var paramXML in methodXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    param.DefaultValue = paramXML.Element("DefaultValue").Value;
                    method.Parameters.Add(param);
                }
                model.Actions.Add(method);
            }
            foreach (var constructorXML in itemXML.Element("Actions").Elements("Constructor"))
            {
                ConstructorViewModel constructor = new ConstructorViewModel(model);
                constructor.Name = constructorXML.Element("Name").Value;
                constructor.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), constructorXML.Element("Visibility").Value);
                foreach (var paramXML in constructorXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    param.DefaultValue = paramXML.Element("DefaultValue").Value;
                    constructor.Parameters.Add(param);
                }
                model.Actions.Add(constructor);
            }
            return model;
        }

        private static StructViewModel DeserializeStruct(XElement itemXML)
        {
            StructViewModel model = new StructViewModel();
            model.Name = itemXML.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                DataService.Instance.ProvideName(model);
            }
            model.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), itemXML.Element("Visibility").Value);
            foreach (var attrXML in itemXML.Element("Attributes").Elements("Attribute"))
            {
                FieldViewModel attribute = new FieldViewModel(model);
                attribute.Name = attrXML.Element("Name").Value;
                attribute.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), attrXML.Element("Visibility").Value);
                attribute.IsStatic = bool.Parse(attrXML.Element("IsStatic").Value);
                attribute.Type = attrXML.Element("Type").Value;
                attribute.DefaultValue = attrXML.Element("DefaultValue").Value;
                model.Attributes.Add(attribute);
            }
            foreach (var propXML in itemXML.Element("Attributes").Elements("Property"))
            {
                PropertyViewModel property = new PropertyViewModel(model);
                property.Name = propXML.Element("Name").Value;
                property.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), propXML.Element("Visibility").Value);
                property.IsStatic = bool.Parse(propXML.Element("IsStatic").Value);
                property.IsAbstract = bool.Parse(propXML.Element("IsAbstract").Value);
                property.IsGet = bool.Parse(propXML.Element("IsGet").Value);
                property.IsSet = bool.Parse(propXML.Element("IsSet").Value);
                property.Type = propXML.Element("Type").Value;
                property.DefaultValue = propXML.Element("DefaultValue").Value;
                model.Attributes.Add(property);
            }
            foreach (var methodXML in itemXML.Element("Actions").Elements("Method"))
            {
                MethodViewModel method = new MethodViewModel(model);
                method.Name = methodXML.Element("Name").Value;
                method.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), methodXML.Element("Visibility").Value);
                method.IsStatic = bool.Parse(methodXML.Element("IsStatic").Value);
                method.Type = methodXML.Element("Type").Value;
                foreach (var paramXML in methodXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    param.DefaultValue = paramXML.Element("DefaultValue").Value;
                    method.Parameters.Add(param);
                }
                model.Actions.Add(method);
            }
            foreach (var methodXML in itemXML.Element("Actions").Elements("Constructor"))
            {
                ConstructorViewModel method = new ConstructorViewModel(model);
                method.Name = methodXML.Element("Name").Value;
                method.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), methodXML.Element("Visibility").Value);
                foreach (var paramXML in methodXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    param.DefaultValue = paramXML.Element("DefaultValue").Value;
                    method.Parameters.Add(param);
                }
                model.Actions.Add(method);
            }
            return model;
        }

        private static InterfaceViewModel DeserializeInterface(XElement itemXML)
        {
            InterfaceViewModel model = new InterfaceViewModel();
            model.Name = itemXML.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                DataService.Instance.ProvideName(model);
            }
            model.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), itemXML.Element("Visibility").Value);
            foreach (var propXML in itemXML.Element("Attributes").Elements("Property"))
            {
                PropertyViewModel property = new PropertyViewModel(model);
                property.Name = propXML.Element("Name").Value;
                property.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), propXML.Element("Visibility").Value);
                property.IsStatic = bool.Parse(propXML.Element("IsStatic").Value);
                property.IsAbstract = bool.Parse(propXML.Element("IsAbstract").Value);
                property.IsGet = bool.Parse(propXML.Element("IsGet").Value);
                property.IsSet = bool.Parse(propXML.Element("IsSet").Value);
                property.Type = propXML.Element("Type").Value;
                property.DefaultValue = propXML.Element("DefaultValue").Value;
                model.Attributes.Add(property);
            }
            foreach (var methodXML in itemXML.Element("Actions").Elements("Method"))
            {
                MethodViewModel method = new MethodViewModel(model);
                method.Name = methodXML.Element("Name").Value;
                method.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), methodXML.Element("Visibility").Value);
                method.IsStatic = bool.Parse(methodXML.Element("IsStatic").Value);
                method.Type = methodXML.Element("Type").Value;
                foreach (var paramXML in methodXML.Element("Parameters").Elements("Parameter"))
                {
                    ParameterViewModel param = new ParameterViewModel();
                    param.Name = paramXML.Element("Name").Value;
                    param.Type = paramXML.Element("Type").Value;
                    param.DefaultValue = paramXML.Element("DefaultValue").Value;
                    method.Parameters.Add(param);
                }
                model.Actions.Add(method);
            }
            return model;
        }


        private static EnumViewModel DeserializeEnum(XElement itemXML)
        {
            EnumViewModel model = new EnumViewModel();
            model.Name = itemXML.Element("Name").Value;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                DataService.Instance.ProvideName(model);
            }
            model.Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), itemXML.Element("Visibility").Value);
            foreach (var childXML in itemXML.Element("EnumChildren").Elements("EnumChild"))
            {
                EnumChildViewModel child = new EnumChildViewModel();
                child.Name = childXML.Element("Name").Value;
                child.Value = childXML.Element("Value").Value;
                model.EnumChildren.Add(child);
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
                    MessageBox.MessageBox.Show("Ошибка", e.Message, MessageBox.MessageBoxButtons.Ok);
                }
            }

            return null;
        }
        private void DeleteCurrentSelection()
        {
            foreach (Connection connection in SelectionService.Selection.OfType<Connection>())
            {
                connection.Release();
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
                (item.Content as IEntry).ReleaseData();
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
                                                            SerializeContent(x.Content)
                                                        ))));
            return serializedItems;
        }

        XElement SerializeContent(object content)
        {
            if (content is ClassViewModel c)
            {
                return SerializeClass(c);
            }
            if (content is InterfaceViewModel i)
            {
                return SerializeInterface(i);
            }
            if (content is StructViewModel s)
            {
                return SerializeStruct(s);
            }
            if (content is EnumViewModel e)
            {
                return SerializeEnum(e);
            }
            return new XElement("NoContent");
        }

        XElement SerializeClass(ClassViewModel model)
        {
            XElement serializedClass = new XElement("Class",
                                                     new XElement("Name", model.Name),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("IsStatic", model.IsStatic),
                                                     new XElement("IsAbstract", model.IsAbstract),
                                                     new XElement("Attributes",
                                                        model.Attributes.OfType<FieldViewModel>().Select(a =>
                                                        new XElement("Attribute",
                                                            new XElement("Name", a.Name),
                                                            new XElement("Visibility", a.Visibility),
                                                            new XElement("Type", a.Type),
                                                            new XElement("IsStatic", a.IsStatic),
                                                            new XElement("DefaultValue", a.DefaultValue)
                                                        )),
                                                        model.Attributes.OfType<PropertyViewModel>().Select(p =>
                                                        new XElement("Property",
                                                            new XElement("Name", p.Name),
                                                            new XElement("Visibility", p.Visibility),
                                                            new XElement("Type", p.Type),
                                                            new XElement("IsStatic", p.IsStatic),
                                                            new XElement("IsAbstract", p.IsAbstract),
                                                            new XElement("IsGet", p.IsGet),
                                                            new XElement("IsSet", p.IsSet),
                                                            new XElement("DefaultValue", p.DefaultValue)
                                                        ))),
                                                     new XElement("Actions",
                                                        model.Actions.OfType<MethodViewModel>().Select(m =>
                                                        new XElement("Method",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Type", m.Type),
                                                            new XElement("IsStatic", m.IsStatic),
                                                            new XElement("IsAbstract", m.IsAbstract),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Type", p.Type),
                                                                    new XElement("DefaultValue", p.DefaultValue)
                                                        ))))),
                                                        model.Actions.OfType<ConstructorViewModel>().Select(m =>
                                                        new XElement("Constructor",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Type", p.Type),
                                                                    new XElement("DefaultValue", p.DefaultValue)
                                                         )))
                                                     ))));
            return serializedClass;
        }

        XElement SerializeInterface(InterfaceViewModel model)
        {
            XElement serializedClass = new XElement("Interface",
                                                     new XElement("Name", model.Name),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("Attributes",
                                                        model.Attributes.OfType<PropertyViewModel>().Select(p =>
                                                        new XElement("Property",
                                                            new XElement("Name", p.Name),
                                                            new XElement("Visibility", p.Visibility),
                                                            new XElement("Type", p.Type),
                                                            new XElement("IsStatic", p.IsStatic),
                                                            new XElement("IsAbstract", p.IsAbstract),
                                                            new XElement("IsGet", p.IsGet),
                                                            new XElement("IsSet", p.IsSet),
                                                            new XElement("DefaultValue", p.DefaultValue)
                                                        ))),
                                                     new XElement("Actions",
                                                        model.Actions.OfType<MethodViewModel>().Select(m =>
                                                        new XElement("Method",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Type", m.Type),
                                                            new XElement("IsStatic", m.IsStatic),
                                                            new XElement("IsAbstract", m.IsAbstract),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Type", p.Type),
                                                                    new XElement("DefaultValue", p.DefaultValue)
                                                        )))))));
            return serializedClass;
        }

        XElement SerializeStruct(StructViewModel model)
        {
            XElement serializedClass = new XElement("Struct",
                                                     new XElement("Name", model.Name),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("Attributes",
                                                        model.Attributes.OfType<FieldViewModel>().Select(a =>
                                                        new XElement("Attribute",
                                                            new XElement("Name", a.Name),
                                                            new XElement("Visibility", a.Visibility),
                                                            new XElement("Type", a.Type),
                                                            new XElement("IsStatic", a.IsStatic),
                                                            new XElement("DefaultValue", a.DefaultValue)
                                                        )),
                                                        model.Attributes.OfType<PropertyViewModel>().Select(p =>
                                                        new XElement("Property",
                                                            new XElement("Name", p.Name),
                                                            new XElement("Visibility", p.Visibility),
                                                            new XElement("Type", p.Type),
                                                            new XElement("IsStatic", p.IsStatic),
                                                            new XElement("IsAbstract", p.IsAbstract),
                                                            new XElement("IsGet", p.IsGet),
                                                            new XElement("IsSet", p.IsSet),
                                                            new XElement("DefaultValue", p.DefaultValue)
                                                        ))),
                                                     new XElement("Actions",
                                                        model.Actions.OfType<MethodViewModel>().Select(m =>
                                                        new XElement("Method",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Type", m.Type),
                                                            new XElement("IsStatic", m.IsStatic),
                                                            new XElement("IsAbstract", m.IsAbstract),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Type", p.Type),
                                                                    new XElement("DefaultValue", p.DefaultValue)
                                                        ))))),
                                                        model.Actions.OfType<ConstructorViewModel>().Select(m =>
                                                        new XElement("Constructor",
                                                            new XElement("Name", m.Name),
                                                            new XElement("Visibility", m.Visibility),
                                                            new XElement("Parameters",
                                                                m.Parameters.Select(p =>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Type", p.Type),
                                                                    new XElement("DefaultValue", p.DefaultValue)
                                                         )))
                                                     ))));
            return serializedClass;
        }

        XElement SerializeEnum(EnumViewModel model)
        {
            XElement serializedClass = new XElement("Enum",
                                                     new XElement("Name", model.Name),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("EnumChildren",
                                                        model.EnumChildren.Select(a =>
                                                        new XElement("EnumChild",
                                                            new XElement("Name", a.Name),
                                                            new XElement("Value", a.Value)
                                                        ))));
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
                                        ))),
                                      SerializeConnectionData(c.ConnectionViewModel.ConnectionData))
                                  ));

            return serializedConnections;
        }

        XElement SerializeConnectionData(IConnectionData connectionData)
        {
            if (connectionData is AggregationDataViewModel ag)
            {
                return new XElement("ConnectionData",
                    new XElement("AggregatedAttribute", ag.AggregatedAttribute?.ToString()),
                    new XElement("AggregatedMethod", ag.AggregatedAction?.ToString())
                    );
            }
            if (connectionData is CompositionDataViewModel c)
            {
                return new XElement("ConnectionData",
                    new XElement("ComposedAttribute", c.ComposedAttribute?.ToString())
                    );
            }
            if (connectionData is DependencyDataViewModel d)
            {
                return new XElement("ConnectionData",
                    new XElement("DependencedMethod", d.DependencedAction?.ToString())
                    );
            }
            return new XElement("ConnectionData");
        }

        void SaveFile(XElement xElement)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Class Designer Files (*.classdsgn)|*.classdsgn|XML-файлы (*.xml)|*.xml|Изображения (*.png)|*.png";
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    if (Path.GetExtension(saveFile.FileName) == ".png")
                    {
                        ToImageSource(this, saveFile.FileName);
                    }
                    else
                    {
                        xElement.Save(saveFile.FileName);
                    }
                    MessageBox.MessageBox.Show("Успех", "Файл сохранен", MessageBox.MessageBoxButtons.Ok);
                }
                catch (Exception ex)
                {
                    MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok);
                }
            }
        }

        public static void ToImageSource(Canvas canvas, string filename)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)canvas.DesiredSize.Width, (int)canvas.DesiredSize.Height, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.DesiredSize.Width, (int)canvas.DesiredSize.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.DesiredSize.Width, (int)canvas.DesiredSize.Height)));
            bmp.Render(canvas);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }


        public void UpdateZIndex()
        {
            List<Connection> connections = this.Children.OfType<Connection>().OrderBy(x => Canvas.GetZIndex(x)).ToList();
            List<DesignerItem> designerItems = this.Children.OfType<DesignerItem>().OrderBy(x => Canvas.GetZIndex(x)).ToList();

            int i = 0;
            for (i = 0; i < connections.Count; i++)
            {
                Canvas.SetZIndex(connections[i], i);
            }
            for (int j = 0; j < designerItems.Count; j++)
            {
                Canvas.SetZIndex(designerItems[j], j+i+1);
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
