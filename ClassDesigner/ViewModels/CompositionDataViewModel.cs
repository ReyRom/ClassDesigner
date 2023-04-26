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

        public AttributeViewModel composedAttribute { get; set; }
        public AttributeViewModel ComposedAttribute
        {
            get => composedAttribute; set
            {
                composedAttribute = value;
                OnPropertyChanged(nameof(ComposedAttribute));
            }
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
            var attribute = new AttributeViewModel();
            attribute.Type = Target.Name;
            (Target as IHaveFields).Attributes.Add(attribute);
            ComposedAttribute = attribute;
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
