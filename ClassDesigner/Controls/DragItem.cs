using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace ClassDesigner.Controls
{
    public class DragItem:ContentControl
    {
        static DragItem()
        {
            //FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
            //    typeof(DragItem), new FrameworkPropertyMetadata(typeof(DragItem)));
        }

        private Point? dragStartPoint = null;

        public static readonly DependencyProperty DragContentProperty =
            DependencyProperty.RegisterAttached("DragContent", typeof(string), typeof(DragItem));
        public string DragContent
        {
            set
            {
                SetValue(DragContentProperty, value);
            }
            get
            {
                return (string)GetValue(DragContentProperty);
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            dragStartPoint = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            if (this.dragStartPoint.HasValue)
            {
                DragDrop.DoDragDrop(this, DragContent, DragDropEffects.Copy);

                e.Handled = true;
            }
        }
    }
}
