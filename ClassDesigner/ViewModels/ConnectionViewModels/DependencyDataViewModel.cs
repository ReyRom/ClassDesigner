using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    class DependencyDataViewModel : ViewModelBase, IConnectionData
    {
        public DependencyDataViewModel(IEntry source, IEntry target)
        {
            Source = source;
            Target = target;
            ValidateSource();
        }
        private IAction dependencedMethod;
        public IAction DependencedMethod
        {
            get => dependencedMethod; set
            {
                if (dependencedMethod is not null)
                {
                    dependencedMethod.PropertyChanged -= DependencedMethod_PropertyChanged;
                }
                dependencedMethod = value;
                if (!dependencedMethod.Parameters.Any(x=>x.Type == Target.Name))
                {
                    dependencedMethod.Parameters.Add(new ParameterViewModel() { Name = Target.Name.ToLower(), Type = Target.Name });
                }
                dependencedMethod.PropertyChanged += DependencedMethod_PropertyChanged;
                OnPropertyChanged(nameof(DependencedMethod));
            }
        }

        private void DependencedMethod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid && isValidSource;

        private bool isValidSource = true;
        public bool IsValidSource => isValidSource;

        Command addDependencedMethod;
        public Command AddDependencedMethod => addDependencedMethod ??= new Command(obj =>
        {
            var method = new MethodViewModel(Source);
            var paramether = new ParameterViewModel();
            paramether.Type = Target.Name;
            method.Parameters.Add(paramether);
            (Source as IHaveActions).Actions.Add(method);
            DependencedMethod = method;
        });

        public void ValidateSource()
        {
            isValidSource = Source is IHaveActions;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            isValid = dependencedMethod is not null && dependencedMethod.Parameters.Any(x => x.Type == Target.Name);
        }
    }
}
