using ClassDesigner.ViewModels;
using ClassDesigner.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<DesignerItem> designerItems = this.SelectionService.Selection.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.SelectionService.Selection.OfType<Connection>();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.Selection.Count > 0;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.SelectionService.ClearSelection();
        }


        XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                                    designerItems.Select(x => 
                                                    new XElement("Item",
                                                        new XElement("Left", Canvas.GetLeft(x)),
                                                        new XElement("Top", Canvas.GetTop(x)),
                                                        new XElement("Width", x.Width),
                                                        new XElement("Heigth", x.Height),
                                                        new XElement("ID", x.ID),
                                                        new XElement("ZIndex", Canvas.GetZIndex(x)),
                                                        new XElement("Content",
                                                            SerializeClass(((Class)x.Content).DataContext as ClassViewModel)
                                                        ))));
            return serializedItems;
        }


        XElement SerializeClass(ClassViewModel model)
        {
            XElement serializedClass = new XElement("Class",
                                                     new XElement("Header", model.Header),
                                                     new XElement("Visibility", model.Visibility),
                                                     new XElement("IsStatic", model.IsStatic),
                                                     new XElement("IsAbstrct", model.IsAbstract),
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
                                                                m.Parameters.Select(p=>
                                                                new XElement("Parameter",
                                                                    new XElement("Name", p.Name),
                                                                    new XElement("Name", p.Type)
                                                                ))
                                                        )))),
                                                     new XElement("Stereotypes",
                                                        model.Stereotypes.Select(s =>
                                                        new XElement("Stereotype", s))
                                                     ));
            return serializedClass;
        }

        XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("RelationType", connection.ConnectionViewModel.RelationType),
                                      new XElement("ZIndex", Canvas.GetZIndex(connection)),
                                      new XElement("Nodes",
                                        connection.Nodes.Select(n=>
                                        new XElement("Node",
                                            new XElement("X", n.Point.X),
                                            new XElement("Y", n.Point.Y),
                                            new XElement("ID", n.Id)
                                        )
                                     ))
                                  ));

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
    }
}
