using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class StructViewModel : ViewModelBase, IEntry, IHaveMethods, IHaveFields, IHaveProperties
    {
        private string name = "Struct";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private VisibilityType visibility = VisibilityType.Public;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        public ObservableCollection<IField> Attributes { get; set; } = new ObservableCollection<IField>();
        public ObservableCollection<IMethod> Methods { get; set; } = new ObservableCollection<IMethod>();

        private int attributeCounter = 0;
        private int propertyCounter = 0;
        private int methodCounter = 0;

        Command addAttributeCommand;
        public Command AddAttributeCommand
        {
            get => addAttributeCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new AttributeViewModel() { Name = "attribute" + ++attributeCounter, Parent = this });
            });
        }

        Command removeAttributeCommand;
        public Command RemoveAttributeCommand
        {
            get => removeAttributeCommand ??= new Command(obj =>
            {
                this.Attributes.Remove(obj as AttributeViewModel);
            });
        }

        Command addPropertyCommand;
        public Command AddPropertyCommand
        {
            get => addPropertyCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new PropertyViewModel() { Name = "Attribute" + ++propertyCounter, Parent = this });
            });
        }


        Command removePropertyCommand;
        public Command RemovePropertyCommand
        {
            get => removePropertyCommand ??= new Command(obj =>
            {
                this.Attributes.Remove(obj as PropertyViewModel);
            });
        }


        Command addMethodCommand;
        public Command AddMethodCommand
        {
            get => addMethodCommand ?? (addMethodCommand = new Command(obj =>
            {
                this.Methods.Add(new MethodViewModel() { Name = "Method" + ++methodCounter, Parent = this });
            }));
        }

        Command removeMethodCommand;
        public Command RemoveMethodCommand
        {
            get => removeMethodCommand ??= new Command(obj =>
            {
                this.Methods.Remove(obj as MethodViewModel);
            });
        }
        //Command openPropertiesCommand;
        //public Command OpenPropertiesCommand
        //{
        //    get => openPropertiesCommand ??= new Command(obj =>
        //    {
        //        PropertiesService.Instance.SelectedCollection = obj;

        //        PropertiesCollectionWindow window = new PropertiesCollectionWindow();

        //        window.ShowDialog();

        //        PropertiesService.Instance.SelectedCollection = null;
        //    });
        //}
    }
}
