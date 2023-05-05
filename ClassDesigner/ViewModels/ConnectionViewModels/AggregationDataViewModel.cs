using ClassDesigner.Helping;
using ClassDesigner.Helping.Services;
using System;
using System.Linq;
using System.Text;

namespace ClassDesigner.ViewModels
{
    internal class AggregationDataViewModel : ViewModelBase, IConnectionData
    {
        public AggregationDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            aggregationError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(aggregationError);

            Validate();
        }

        private IAction aggregatedAction;
        public IAction AggregatedAction
        {
            get => aggregatedAction; set
            {
                if (aggregatedAction is not null)
                {
                    aggregatedAction.PropertyChanged -= AggregatedMethod_PropertyChanged;
                }
                aggregatedAction = value;
                if (!aggregatedAction.Parameters.Any(x => x.Type == Source.Name))
                {
                    aggregatedAction.Parameters.Add(new ParameterViewModel() { Name = Source.Name.ToLower(), Type = Source.Name });
                }
                aggregatedAction.PropertyChanged += AggregatedMethod_PropertyChanged;
                OnPropertyChanged(nameof(AggregatedAction));
                Validate();
            }
        }

        private void AggregatedMethod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IAttribute aggregatedAttribute;
        public IAttribute AggregatedAttribute
        {
            get => aggregatedAttribute; set
            {
                if (aggregatedAttribute is not null)
                {
                    aggregatedAttribute.PropertyChanged -= AggregatedAttribute_PropertyChanged;
                }
                aggregatedAttribute = value;
                if (aggregatedAttribute.Type != Source.Name)
                {
                    aggregatedAttribute.Type = Source.Name;
                }
                aggregatedAttribute.PropertyChanged += AggregatedAttribute_PropertyChanged;
                OnPropertyChanged(nameof(AggregatedAttribute));
                Validate();
            }
        }

        private void AggregatedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IErrorProvider ErrorSource { get; }
        public IEntry Source { get; }
        public IEntry Target { get; }

        Command addAggregatedField;
        public Command AddAggregatedField => addAggregatedField ??= new Command(obj =>
        {
            var attribute = new FieldViewModel(Target);
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            AggregatedAttribute = attribute;
        });


        Command addAggregatedProperty;

        public Command AddAggregatedProperty => addAggregatedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel(Target);
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            AggregatedAttribute = property;
        });

        Command addAggregatedConstructor;
        public Command AddAggregatedConstructor => addAggregatedConstructor ??= new Command(obj =>
        {
            var method = new ConstructorViewModel(Target);
            var paramether = new ParameterViewModel();
            paramether.Type = Source.Name;
            method.Parameters.Add(paramether);
            (Target as IHaveActions).Actions.Add(method);
            AggregatedAction = method;
        });

        Command addAggregatedMethod;
        public Command AddAggregatedMethod => addAggregatedMethod ??= new Command(obj =>
        {
            var method = new MethodViewModel(Target);
            var paramether = new ParameterViewModel();
            paramether.Type = Source.Name;
            method.Parameters.Add(paramether);
            (Target as IHaveActions).Actions.Add(method);
            AggregatedAction = method;
        });

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

        public bool ValidateSource()
        {
            return IsValidSource = (Target is IHaveFields || Target is IHaveProperties) && Target is IHaveActions;
        }

        ErrorViewModel aggregationError;

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Цель агрегации должна иметь действия и атрибуты");
            }
            else
            {
                if (!ValidateAttribute())
                {
                    sb.AppendLine("Не указан атрибут агрегации, или указан некорректный");
                }
                if (!ValidateAction())
                {
                    sb.AppendLine("Не указано действие агрегации, или указано некорректное");
                }
            }
            aggregationError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateAttribute()
        {
            var isValid = aggregatedAttribute is not null
                        && aggregatedAttribute.Type == Source.Name;
            return isValid;
        }

        private bool ValidateAction()
        {
            var isValid = aggregatedAction is not null
                        && aggregatedAction.Parameters.Any(x => x.Type == Source.Name);
            return isValid;
        }

        public void ReleaseData()
        {
            if (aggregationError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(aggregationError);
                aggregationError = null;
            }
        }
    }
}
