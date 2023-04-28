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
    internal class EntryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ClassTemplate { get; set; }
        public DataTemplate InterfaceTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate StructTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ClassViewModel) return ClassTemplate;
            if (item is InterfaceViewModel) return InterfaceTemplate;
            if (item is EnumViewModel) return EnumTemplate;
            if (item is StructViewModel) return StructTemplate;
            else return DefaultTemplate;
        }
    }
}
