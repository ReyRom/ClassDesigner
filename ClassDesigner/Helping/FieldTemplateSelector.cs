using ClassDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClassDesigner.Helping
{
    internal class FieldTemplateSelector:DataTemplateSelector
    {
        public DataTemplate AttributeTemplate { get; set; }
        public DataTemplate PropertyTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AttributeViewModel) return AttributeTemplate;
            if (item is PropertyViewModel) return PropertyTemplate;
            else return DefaultTemplate;
        }
    }
}
