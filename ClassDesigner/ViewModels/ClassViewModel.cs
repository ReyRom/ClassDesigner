using ClassDesigner.Helping;
using ClassDesigner.Models;
using ClassDesigner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClassDesigner.ViewModels
{
    public class ClassViewModel : ViewModelBase, IEntry, IHaveMethods, IHaveFields
    {
        private string name = "Class";
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
        
        private bool isStatic = false;
        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;
                OnPropertyChanged(nameof(IsStatic));
            }
        }

        private bool isAbstract = false;
        public bool IsAbstract
        {
            get => isAbstract; set
            {
                isAbstract = value;
                OnPropertyChanged(nameof(IsAbstract));
            }
        }
        public ObservableCollection<IField> Attributes { get; set; } = new ObservableCollection<IField>(); //{ new AttributeViewModel() };
        //public ObservableCollection<Stereotype> Stereotypes { get; set; } = new ObservableCollection<Stereotype>() ;
        //public ClassStereotypes Stereotypes { get; set; } = new ClassStereotypes();
        public ObservableCollection<MethodViewModel> Methods { get; set; } = new ObservableCollection<MethodViewModel>(); //{ new MethodViewModel() };

        Command addAttributeCommand;
        public Command AddAttributeCommand
        {
            get => addAttributeCommand ?? (addAttributeCommand = new Command(obj =>
            {
                this.Attributes.Add(new AttributeViewModel());
            }));
        }

        Command addPropertyCommand;
        public Command AddPropertyCommand
        {
            get => addPropertyCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new PropertyViewModel());
            });
        }

        Command addMethodCommand;
        public Command AddMethodCommand
        {
            get => addMethodCommand ?? (addMethodCommand = new Command(obj =>
            {
                this.Methods.Add(new MethodViewModel());
            }));
        }
 

        //public Command AddStereotypeCommand
        //{
        //    get => addStereotypeCommand ?? (addStereotypeCommand = new Command(obj =>
        //    {
        //        if (this.Stereotypes.Contains((Stereotype)obj))
        //        {
        //            this.Stereotypes.Remove((Stereotype)obj);
        //        }
        //        else
        //        {
        //            this.Stereotypes.Add((Stereotype)obj);
        //        }
        //    }));
        //}

        Command openPropertiesCommand;
        public Command OpenPropertiesCommand
        {
            get => openPropertiesCommand ??= new Command(obj =>
            {
                PropertiesService.Instance.SelectedCollection = obj;

                PropertiesCollectionWindow window = new PropertiesCollectionWindow();

                window.ShowDialog();

                PropertiesService.Instance.SelectedCollection = null;
            });
        }
    }
}
