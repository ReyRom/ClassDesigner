using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class RealizationDataViewModel:ViewModelBase,IConnectionData
    {
        public RealizationDataViewModel(IEntry source, IEntry target)
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
            isValidSource = Target is InterfaceViewModel && Source is ClassViewModel;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            foreach (var item in Fields)
            {
                if (!isValid) break;
                isValid = (Source as IHaveAttributes).Attributes.Contains(item);
            }
            foreach (var item in Methods)
            {
                if (!isValid) break;
                isValid = (Source as IHaveMethods).Methods.Contains(item);
            }
            OnPropertyChanged(nameof(IsValid));
        }
        Command realizeFieldCommand;
        public Command RealizeFieldCommand => realizeFieldCommand ??= new Command(obj=>
        {
             (Source as IHaveAttributes).Attributes.Add(obj as IField);
        }, obj => !(Source as IHaveAttributes).Attributes.Contains(obj as IField));
        Command realizeMethodCommand;
        public Command RealizeMethodCommand => realizeMethodCommand ??= new Command(obj =>
        {
          (Source as IHaveMethods).Methods.Add(obj as IMethod);
        }, obj => !(Source as IHaveMethods).Methods.Contains(obj as IMethod));
    }
}
