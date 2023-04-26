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
    public class EnumViewModel : ViewModelBase
    {
        private string name = "Enum";
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

        private ObservableCollection<EnumChildViewModel> enumChildren = new ObservableCollection<EnumChildViewModel>();
        public ObservableCollection<EnumChildViewModel> EnumChildren { get => enumChildren; set => enumChildren = value; }
        

        Command addEnumChildCommand;
        public Command AddEnumChildCommand
        {
            get => addEnumChildCommand ?? (addEnumChildCommand = new Command(obj =>
            {
                this.EnumChildren.Add(new EnumChildViewModel());
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
