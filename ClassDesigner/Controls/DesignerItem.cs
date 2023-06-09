﻿using ClassDesigner.Helping;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClassDesigner.Controls
{
    public class DesignerItem : ContentControl, ISelectable
    {
        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
        }

        public DesignerItem()
            : this(Guid.NewGuid())
        {
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveSelection(this);

                        foreach (var item in designer.GetConnections(this))
                        {
                            designer.SelectionService.RemoveSelection(item);
                        }
                    }
                    else
                    {
                        designer.SelectionService.AddSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }
                //Focus();
            }
            e.Handled = true;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template is not null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter is not null)
                {
                    if (VisualTreeHelper.GetChildrenCount(contentPresenter)>0) 
                    {
                        UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                        if (contentVisual is not null)
                        {
                            DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                            if (thumb is not null)
                            {
                                ControlTemplate template =
                                    DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    thumb.Template = template;
                            }
                        }
                    }
                }
            }
        }
    }
}
