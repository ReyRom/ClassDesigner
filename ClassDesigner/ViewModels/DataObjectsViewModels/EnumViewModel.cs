using ClassDesigner.Helping;
using ClassDesigner.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClassDesigner.ViewModels
{
    public class EnumViewModel : ViewModelBase, IEntry, IErrorProvider
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

        ErrorViewModel enumError;

        public EnumViewModel()
        {
            enumError = new ErrorViewModel() { Source = this };
            ErrorService.Instance.ObservableErrors.Add(enumError);

            EnumChildren.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            Validate();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Validate();
        }

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();

            if (!ValidateItem())
            {
                sb.AppendLine("Перечисление не может содержать двух одинаковых значений");
            }

            enumError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateItem()
        {
            var isValid = !EnumChildren.GroupBy(x => x.Name).Any(g => g.Count() > 1);
            return isValid;
        }

        public void ReleaseData()
        {
            if (enumError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(enumError);
                enumError = null;
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
