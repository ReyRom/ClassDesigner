using ClassDesigner.Helping;
using System.Linq;
using System.Text;

namespace ClassDesigner.ViewModels
{
    class CompositionDataViewModel : ViewModelBase, IConnectionData
    {
        public CompositionDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            compositionError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(compositionError);

            Validate();
        }

        IAttribute composedAttribute;
        public IAttribute ComposedAttribute
        {
            get => composedAttribute; set
            {
                if (composedAttribute is not null)
                {
                    composedAttribute.PropertyChanged -= ComposedAttribute_PropertyChanged;
                }
                composedAttribute = value;
                //if (composedAttribute.Type != Source.Name)
                //{
                //    composedAttribute.Type = Source.Name;
                //}
                composedAttribute.PropertyChanged += ComposedAttribute_PropertyChanged;
                OnPropertyChanged(nameof(ComposedAttribute));
                Validate();
            }
        }

        private void ComposedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }
        private ErrorViewModel compositionError;

        public IErrorProvider ErrorSource { get; }
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

        Command addComposedAttribute;
        public Command AddComposedAttribute => addComposedAttribute ??= new Command(obj =>
        {
            var attribute = new FieldViewModel(Target);
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            ComposedAttribute = attribute;
        }, obj => Target is IHaveFields);


        Command addComposedProperty;
        public Command AddComposedProperty => addComposedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel(Target);
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            ComposedAttribute = property;
        }, obj => Target is IHaveProperties);

        public bool ValidateSource()
        {
            return IsValidSource = Target is IHaveFields || Target is IHaveProperties;
        }
        private bool ValidateAttribute()
        {
            var isValid = composedAttribute is not null;
            if (isValid)
            {
                isValid = composedAttribute.Type == Source.Name || DataService.CheckModifiedType(Source.Name, composedAttribute.Type);
                if (Source is IInheritor i)
                {
                    isValid = isValid || i.Parents.Any(x => x.Name == composedAttribute.Type);
                }
            }

            return isValid;
        }
        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Источником реализации может быть только класс, а целью только интерфейс");
            }
            else
            {
                if (!ValidateAttribute())
                {
                    sb.AppendLine("Не указан атрибут композиции");
                }
            }
            compositionError.Text = sb.ToString().TrimEnd();
        }

        public void ReleaseData()
        {
            if (compositionError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(compositionError);
                compositionError = null;
            }
        }
    }
}
