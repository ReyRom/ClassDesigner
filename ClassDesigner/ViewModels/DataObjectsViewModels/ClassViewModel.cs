using ClassDesigner.Helping;
using ClassDesigner.Helping.Interfaces;
using ClassDesigner.Models;
using ClassDesigner.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClassDesigner.ViewModels
{
    public class ClassViewModel : ViewModelBase, IEntry, IHaveConstructors, IHaveMethods, IHaveFields, IHaveProperties, IInheritable, IInheritor, IErrorProvider
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
                isAbstract = isStatic ? false : isAbstract;
                OnPropertyChanged(nameof(IsStatic));
                OnPropertyChanged(nameof(IsAbstract));
            }
        }

        private bool isAbstract = false;
        public bool IsAbstract
        {
            get => isAbstract; set
            {
                isAbstract = value;
                isStatic = isAbstract ? false : isStatic;
                OnPropertyChanged(nameof(IsAbstract));
                OnPropertyChanged(nameof(IsStatic));
            }
        }

        public ObservableCollection<IAttribute> Attributes { get; set; } = new ObservableCollection<IAttribute>();
        public ObservableCollection<IAction> Actions { get; set; } = new ObservableCollection<IAction>();

        private int attributeCounter = 0;
        private int propertyCounter = 0;
        private int methodCounter = 0;

        Command addFieldCommand;
        public Command AddFieldCommand
        {
            get => addFieldCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new FieldViewModel(this) { Name = "attribute" + ++attributeCounter });
            });
        }

        Command removeFieldCommand;
        public Command RemoveFieldCommand
        {
            get => removeFieldCommand ??= new Command(obj =>
            {
                this.Attributes.Remove(obj as FieldViewModel);
            });
        }

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
            get => addMethodCommand ??= new Command(obj =>
            {
                this.Actions.Add(new MethodViewModel(this) { Name = "Method" + ++methodCounter });
            });
        }

        Command removeMethodCommand;
        public Command RemoveMethodCommand
        {
            get => removeMethodCommand ??= new Command(obj =>
            {
                this.Actions.Remove(obj as MethodViewModel);
            });
        }


        Command addConstructorCommand;
        public Command AddConstructorCommand
        {
            get => addConstructorCommand ??= new Command(obj =>
            {
                this.Actions.Add(new ConstructorViewModel(this));
            });
        }

        Command removeConstructorCommand;
        public Command RemoveConstructorCommand
        {
            get => removeConstructorCommand ??= new Command(obj =>
            {
                this.Actions.Remove(obj as ConstructorViewModel);
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

        private ObservableCollection<IInheritable> parents = new ObservableCollection<IInheritable>();
        public ObservableCollection<IInheritable> Parents
        {
            get => parents; set
            {
                parents = value;
                OnPropertyChanged(nameof(Parents));
            }
        }

        ErrorViewModel classError;

        public ClassViewModel()
        {
            classError = new ErrorViewModel() { Source = this };
            ErrorService.Instance.ObservableErrors.Add(classError);

            Attributes.CollectionChanged += CollectionChanged;
            Actions.CollectionChanged += CollectionChanged;
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

            if (!ValidateAttribute())
            {
                sb.AppendLine("Класс не может содержать двух одинаковых атрибутов");
            }
            if (!ValidateAction())
            {
                sb.AppendLine("Класс не может содержать двух одинаковых методов");
            }

            classError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateAttribute()
        {
            var isValid = !Attributes.GroupBy(x => x.AttributeString).Any(g => g.Count() > 1);
            return isValid;
        }

        private bool ValidateAction()
        {
            var isValid = !Actions.GroupBy(x => x.ActionString).Any(g => g.Count() > 1);
            return isValid;
        }

        public void ReleaseData()
        {
            if (classError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(classError);
                classError = null;
            }
            foreach (IRelease item in Actions)
            {
                item?.ReleaseData();
            }
        }
    }
}
