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
    internal class RelationTemplateSelector:DataTemplateSelector
    {
        public DataTemplate DependencyTemplate { get; set; }
        public DataTemplate AssociationTemplate { get; set; }
        public DataTemplate AggregationTemplate { get; set; }
        public DataTemplate CompositionTemplate { get; set; }
        public DataTemplate GeneralizationTemplate { get; set; }
        public DataTemplate RealizationTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var type = (RelationType)item;
            switch (type)
            {
                case RelationType.Dependency:
                    return DependencyTemplate;
                case RelationType.Association:
                    return AssociationTemplate;
                case RelationType.Aggregation:
                    return AggregationTemplate;
                case RelationType.Composition:
                    return CompositionTemplate;
                case RelationType.Generalization:
                    return GeneralizationTemplate;
                case RelationType.Realization:
                    return RealizationTemplate;
                default:
                    return DefaultTemplate;
            }

        }
    }
}
