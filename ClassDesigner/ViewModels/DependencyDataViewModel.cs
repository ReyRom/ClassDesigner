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
        }
        private MethodViewModel dependencedMethod;
        public MethodViewModel DependencedMethod
        {
            get => dependencedMethod; set
            {
                dependencedMethod = value;
                if (!dependencedMethod.Parameters.Any(x=>x.Type == Target.Name))
                {
                    dependencedMethod.Parameters.Add(new ParameterViewModel() { Name = Target.Name.ToLower(), Type = Target.Name });
                }
                OnPropertyChanged(nameof(DependencedMethod));
            }
        }
        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid;

        Command addDependencedMethod;
        public Command AddDependencedMethod => addDependencedMethod ??= new Command(obj =>
        {
            var method = new MethodViewModel();
            var paramether = new ParameterViewModel();
            paramether.Type = Target.Name;
            method.Parameters.Add(paramether);
            (Source as IHaveMethods).Methods.Add(method);

            DependencedMethod = method;

        });

        public void Validate()
        {
            isValid = true;

        }
    }
}
