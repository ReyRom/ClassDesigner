using ClassDesigner.Controls;
using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AssotiationDataViewModel:ViewModelBase, IConnectionData
    {
        public AssotiationDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            assotiationError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(assotiationError);

            Validate();
        }

        private ErrorViewModel assotiationError;

        IAttribute assotiatedAttribute;
        public IAttribute AssotiatedAttribute
        {
            get => assotiatedAttribute; set
            {
                if (assotiatedAttribute is not null)
                {
                    assotiatedAttribute.PropertyChanged -= AssotiatedAttribute_PropertyChanged;
                }
                assotiatedAttribute = value;
                if (assotiatedAttribute.Type != Source.Name)
                {
                    assotiatedAttribute.Type = Source.Name;
                }
                assotiatedAttribute.PropertyChanged += AssotiatedAttribute_PropertyChanged;
                OnPropertyChanged(nameof(AssotiatedAttribute));
                Validate();
            }
        }

        private void AssotiatedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IErrorProvider ErrorSource { get; }
        public IEntry Source { get; }
        public IEntry Target { get; }

        Command addAssotiatedField;
        public Command AddAssotiatedField => addAssotiatedField ??= new Command(obj =>
        {
            var attribute = new FieldViewModel(Target);
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            AssotiatedAttribute = attribute;
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

        Command addAssotiatedProperty;
        public Command AddAssotiatedProperty => addAssotiatedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel(Target);
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            AssotiatedAttribute = property;
        });

        public bool ValidateSource()
        {
            return IsValidSource = Target is IHaveFields || Target is IHaveProperties;
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
                    sb.AppendLine("Не указан атрибут ассоциации");
                }
            }
            assotiationError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateAttribute()
        {
            var isValid = assotiatedAttribute is not null 
                && assotiatedAttribute.Type == Source.Name;
            return isValid;
        }

        public void ReleaseData()
        {
            if (assotiationError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(assotiationError);
                assotiationError = null;
            }
        }
    }
}
