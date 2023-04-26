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
    class InterfaceViewModel:ViewModelBase, IEntry, IHaveMethods, IHaveProperties
    {
        private string name = "Interface";
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
       
        public ObservableCollection<PropertyViewModel> Properties { get; set; } = new ObservableCollection<PropertyViewModel>(); //{ new AttributeViewModel() };
        public ObservableCollection<MethodViewModel> Methods { get; set; } = new ObservableCollection<MethodViewModel>(); //{ new MethodViewModel() };

        Command addPropertyCommand;
        public Command AddPropertyCommand
        {
            get => addPropertyCommand ?? (addPropertyCommand = new Command(obj =>
            {
                this.Properties.Add(new PropertyViewModel());
            }));
        }

        Command addMethodCommand;
        public Command AddMethodCommand
        {
            get => addMethodCommand ?? (addMethodCommand = new Command(obj =>
            {
                this.Methods.Add(new MethodViewModel());
            }));
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
