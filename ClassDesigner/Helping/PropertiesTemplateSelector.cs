using ClassDesigner.Models;
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
    public class PropertiesTemplateSelector:DataTemplateSelector
    {
        public DataTemplate ClassTemplate { get; set; }
        public DataTemplate ConnectionTemplate { get; set; }
        public DataTemplate MethodTemplate { get; set; }
        public DataTemplate AttributeTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ClassViewModel) return ClassTemplate;
            if (item is ConnectionViewModel) return ConnectionTemplate;
            if (item is AttributeViewModel) return AttributeTemplate;
            if (item is MethodViewModel) return MethodTemplate;
            else return DefaultTemplate;

        }
    }
}
