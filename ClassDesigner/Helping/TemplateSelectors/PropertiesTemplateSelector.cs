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
        public DataTemplate ConstructorTemplate { get; set; }
        public DataTemplate FieldTemplate { get; set; }
        public DataTemplate PropertyTemplate { get; set; }
        public DataTemplate InterfaceTemplate { get; set; }
        public DataTemplate StructTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate EnumChildTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case ClassViewModel:
                    return ClassTemplate;
                case ConnectionViewModel:
                    return ConnectionTemplate;
                case MethodViewModel:
                    return MethodTemplate;
                case ConstructorViewModel: 
                    return ConstructorTemplate;
                case FieldViewModel:
                    return FieldTemplate;
                case PropertyViewModel: 
                    return PropertyTemplate;
                case InterfaceViewModel: 
                    return InterfaceTemplate;
                case StructViewModel: 
                    return StructTemplate;
                case EnumViewModel: 
                    return EnumTemplate;
                case EnumChildViewModel: 
                    return EnumChildTemplate;
                default:
                    return DefaultTemplate;
            }
        }
    }
}
