using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class GeneralizationDataViewModel:ViewModelBase, IConnectionData
    {
        public GeneralizationDataViewModel(IEntry source, IEntry target)
        {
            Source = source;
            Target = target;
            ValidateSource();
        }

        public IEnumerable<IField> Fields => ((IHaveAttributes)Target).Attributes.Where(x => x.Visibility != Models.VisibilityType.Private);
        public IEnumerable<IMethod> Methods => ((IHaveMethods)Target).Methods.Where(x => x.Visibility != Models.VisibilityType.Private);

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid && isValidSource;

        private bool isValidSource = true;
        public bool IsValidSource => isValidSource;

        public void ValidateSource()
        {
            isValidSource = Target is InterfaceViewModel && Source is InterfaceViewModel
                          || Target is ClassViewModel && Source is ClassViewModel;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            
        }
    }
}
