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
    public class InterfaceViewModel:ViewModelBase, IEntry, IHaveActions, IHaveProperties, IInheritable, IInheritor
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
       
        public ObservableCollection<IAttribute> Attributes { get; set; } = new ObservableCollection<IAttribute>(); 
        public ObservableCollection<IAction> Actions { get; set; } = new ObservableCollection<IAction>();


        private int propertyCounter = 0;
        private int methodCounter = 0;

        Command addPropertyCommand;
        public Command AddPropertyCommand
        {
            get => addPropertyCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new PropertyViewModel(this) { Name = "Attribute" + ++propertyCounter });
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
                this.Actions.Add(new MethodViewModel(this) { Name = "Method" + ++methodCounter});
            }));
        }

        Command removeMethodCommand;
        public Command RemoveMethodCommand
        {
            get => removeMethodCommand ??= new Command(obj =>
            {
                this.Actions.Remove(obj as MethodViewModel);
            });
        }

        private ObservableCollection<IInheritable> parents = new ObservableCollection<IInheritable>();
        public ObservableCollection<IInheritable> Parents
        {
            get => parents; set
            {
                parents = value;
                OnPropertyChanged(nameof(Parents));
            }
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
