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
    public class AttributeTemplateSelector:DataTemplateSelector
    {
        public DataTemplate FieldTemplate { get; set; }
        public DataTemplate PropertyTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is FieldViewModel) return FieldTemplate;
            if (item is PropertyViewModel) return PropertyTemplate;
            else return DefaultTemplate;
        }
    }
    public class ActionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MethodTemplate { get; set; }
        public DataTemplate ConstructorTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MethodViewModel) return MethodTemplate;
            if (item is ConstructorViewModel) return ConstructorTemplate;
            else return DefaultTemplate;
        }
    }
}
