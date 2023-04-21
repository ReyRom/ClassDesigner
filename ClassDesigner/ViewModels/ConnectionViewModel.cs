using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ConnectionViewModel:ViewModelBase
    {
        public ConnectionViewModel()
        {
            RelationType = SettingsService.Instance.RelationType;
        }

        public ClassViewModel SourceClass { get; set; }
        public ClassViewModel TargetClass { get; set;}
        public RelationType RelationType { get; set; }

        public ObservableCollection<MethodViewModel> InheritedMethods { get; set; }
        public ObservableCollection<AttributeViewModel> InheritedAttributes { get; set; }

        public ObservableCollection<MethodViewModel> RealizedMethods { get; set; }
        public ObservableCollection<AttributeViewModel> RealizedAttributes { get; set; }

        public AttributeViewModel AggregatedAttribute { get; set; }
        public MethodViewModel AggregatedConstructor { get; set; }

        public AttributeViewModel ComposedAttribute { get; set; }

        public MethodViewModel DependencedMethod { get; set; }
    }
}
