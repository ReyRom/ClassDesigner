using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    class CompositionDataViewModel:ViewModelBase, IConnectionData
    {
        public CompositionDataViewModel(IEntry source, IEntry target)
        {
            Source = source;
            Target = target;
            ValidateSource();
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
                if (composedAttribute.Type != Source.Name)
                {
                    composedAttribute.Type = Source.Name;
                }
                composedAttribute.PropertyChanged += ComposedAttribute_PropertyChanged;
                OnPropertyChanged(nameof(ComposedAttribute));
            }
        }

        private void ComposedAttribute_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate();
        }

        public IEntry Source { get; }
        public IEntry Target { get; }

        private bool isValid = true;
        public bool IsValid => isValid && isValidSource;

        private bool isValidSource = true;
        public bool IsValidSource => isValidSource;

        Command addComposedAttribute;
        public Command AddComposedAttribute => addComposedAttribute ??= new Command(obj =>
        {
            var attribute = new FieldViewModel(Target);
            attribute.Type = Source.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            ComposedAttribute = attribute;
        });


        Command addComposedProperty;
        public Command AddComposedProperty => addComposedProperty ??= new Command(obj =>
        {
            var property = new PropertyViewModel(Target);
            property.Type = Source.Name;
            (Target as IHaveProperties).Attributes.Add(property);
            ComposedAttribute = property;
        });

        public void ValidateSource()
        {
            isValidSource = Target is IHaveFields || Target is IHaveProperties;
            OnPropertyChanged(nameof(IsValidSource));
            OnPropertyChanged(nameof(IsValid));
        }

        public void Validate()
        {
            isValid = composedAttribute is not null && composedAttribute.Type == Source.Name;
        }

    }
}
