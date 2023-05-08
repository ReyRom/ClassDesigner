using ClassDesigner.Controls;
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
        public DependencyDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            dependencyError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(dependencyError);

            Validate();
        }

        public IErrorProvider ErrorSource { get; }

        private IAction dependencedAction;
        public IAction DependencedAction
        {
            get => dependencedAction; set
            {
                if (dependencedAction is not null)
                {
                    dependencedAction.PropertyChanged -= DependencedMethod_PropertyChanged;
                }
                dependencedAction = value;
                if (!dependencedAction.Parameters.Any(x=>x.Type == Target.Name))
                {
                    dependencedAction.Parameters.Add(new ParameterViewModel() { Name = Target.Name.ToLower(), Type = Target.Name });
                }
                dependencedAction.PropertyChanged += DependencedMethod_PropertyChanged;
                OnPropertyChanged(nameof(DependencedAction));
            }
        }

        private void DependencedMethod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        ErrorViewModel dependencyError;

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValidSource = true;
        public bool IsValidSource
        {
            get
            {
                return isValidSource;
            }
            set
            {
                isValidSource = value;
                OnPropertyChanged(nameof(IsValidSource));
            }
        }

        Command addDependencedMethod;
        public Command AddDependencedMethod => addDependencedMethod ??= new Command(obj =>
        {
            var method = new MethodViewModel(Source);
            var paramether = new ParameterViewModel();
            paramether.Type = Target.Name;
            method.Parameters.Add(paramether);
            (Source as IHaveActions).Actions.Add(method);
            DependencedAction = method;
        });

        public bool ValidateSource()
        {
            return IsValidSource = Source is IHaveActions;
        }

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Источник зависимости должен поддерживать действия");
            }
            else
            {
                if (!ValidateAction())
                {
                    sb.AppendLine("Не указано действие зависимости, или указано некорректное");
                }
            }
            dependencyError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateAction()
        {
            var isValid = dependencedAction is not null 
                && dependencedAction.Parameters.Any(x => x.Type == Target.Name);
            return isValid;
        }

        public void ReleaseData()
        {
            if (dependencyError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(dependencyError);
                dependencyError = null;
            }
        }
    }
}
