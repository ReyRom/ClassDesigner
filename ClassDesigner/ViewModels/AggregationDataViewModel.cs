using ClassDesigner.Helping;
using System.Linq;

namespace ClassDesigner.ViewModels
{
    internal class AggregationDataViewModel : ViewModelBase, IConnectionData
    {
        public AggregationDataViewModel(IEntry source, IEntry target)
        {
            Source = source;
            Target = target;
            ValidateSource();
        }

        private IMethod aggregatedMethod;
        public IMethod AggregatedMethod
        {
            get => aggregatedMethod; set
            {
                if (aggregatedMethod is not null)
                {
                    aggregatedMethod.PropertyChanged -= AggregatedMethod_PropertyChanged;
                }
                aggregatedMethod = value;
                if (!aggregatedMethod.Parameters.Any(x => x.Type == Source.Name))
                {
                    aggregatedMethod.Parameters.Add(new ParameterViewModel() { Name = Source.Name.ToLower(), Type = Source.Name });
                }
                aggregatedMethod.PropertyChanged += AggregatedMethod_PropertyChanged;
                OnPropertyChanged(nameof(AggregatedMethod));
            }
        }

        private void AggregatedMethod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public IField aggregatedAttribute;
        public IField AggregatedAttribute
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
            }
        }

        private void AggregatedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid && isValidSource;

        private bool isValidSource = true;
        public bool IsValidSource => isValidSource;

        Command addAggregatedAttribute;
        public Command AddAggregatedAttribute => addAggregatedAttribute ??= new Command(obj =>
        {
            var attribute = new AttributeViewModel();
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            AggregatedAttribute = attribute;
        });


        Command addAggregatedProperty;

        public Command AddAggregatedProperty => addAggregatedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel();
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            AggregatedAttribute = property;
        });

        Command addAggregatedConstructor;
        public Command AddAggregatedConstructor => addAggregatedConstructor ??= new Command(obj =>
        {
            var method = new ConstructorViewModel();
            var paramether = new ParameterViewModel();
            paramether.Type = Source.Name;
            method.Parameters.Add(paramether);
            (Target as IHaveMethods).Methods.Add(method);
            AggregatedMethod = method;
        });

        Command addAggregatedMethod;
        public Command AddAggregatedMethod => addAggregatedMethod ??= new Command(obj =>
        {
            var method = new MethodViewModel();
            var paramether = new ParameterViewModel();
            paramether.Type = Source.Name;
            method.Parameters.Add(paramether);
            (Target as IHaveMethods).Methods.Add(method);
            AggregatedMethod = method;
        });

        public void ValidateSource()
        {
            isValidSource = (Target is IHaveFields || Target is IHaveProperties) && Target is IHaveMethods;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            isValid = aggregatedAttribute is not null
                        && aggregatedAttribute.Type == Source.Name
                        && aggregatedMethod is not null
                        && aggregatedMethod.Parameters.Any(x => x.Type == Target.Name);
        }

    }
}
