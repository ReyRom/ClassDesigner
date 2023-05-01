using ClassDesigner.Helping;
using ClassDesigner.Models;
using System.Collections.ObjectModel;

namespace ClassDesigner.ViewModels
{
    public class EnumViewModel : ViewModelBase, IEntry
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

        private int childCounter = 0;

        Command addEnumChildCommand;
        public Command AddEnumChildCommand
        {
            get => addEnumChildCommand ?? (addEnumChildCommand = new Command(obj =>
            {
                this.EnumChildren.Add(new EnumChildViewModel() {Name = "value"+ ++childCounter, Parent = this });
            }));
        }


        Command removeEnumChildCommand;
        public Command RemoveEnumChildCommand
        {
            get => removeEnumChildCommand ??= new Command(obj =>
            {
                this.EnumChildren.Remove(obj as EnumChildViewModel);
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
