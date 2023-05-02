using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AssotiationDataViewModel:ViewModelBase, IConnectionData
    {
        public AssotiationDataViewModel(IEntry source, IEntry target)
        {
            Source = source;
            Target = target;
            ValidateSource();
        }

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
            }
        }

        private void AssotiatedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid && isValidSource;

        private bool isValidSource = true;
        public bool IsValidSource => isValidSource;

        Command addAssotiatedField;
        public Command AddAssotiatedField => addAssotiatedField ??= new Command(obj =>
        {
            var attribute = new FieldViewModel(Target);
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            AssotiatedAttribute = attribute;
        });


        Command addAssotiatedProperty;
        public Command AddAssotiatedProperty => addAssotiatedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel(Target);
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            AssotiatedAttribute = property;
        });

        public void ValidateSource()
        {
            isValidSource = Target is IHaveFields || Target is IHaveProperties;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            isValid = assotiatedAttribute is not null && assotiatedAttribute.Type == Source.Name;
        }
    }
}
