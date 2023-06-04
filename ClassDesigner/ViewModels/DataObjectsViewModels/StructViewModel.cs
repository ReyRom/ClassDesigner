using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class StructViewModel : ViewModelBase, IEntry, IHaveConstructors, IHaveMethods, IHaveFields, IHaveProperties, IInheritor, IErrorProvider
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
        public ObservableCollection<IAttribute> Attributes { get; set; } = new ObservableCollection<IAttribute>();
        public ObservableCollection<IAction> Actions { get; set; } = new ObservableCollection<IAction>();

        private int fieldCounter = 0;
        private int propertyCounter = 0;
        private int methodCounter = 0;

        Command addFieldCommand;
        public Command AddFieldCommand
        {
            get => addFieldCommand ??= new Command(obj =>
            {
                this.Attributes.Add(new FieldViewModel(this) { Name = "attribute" + ++fieldCounter });
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
            get => addMethodCommand ?? (addMethodCommand = new Command(obj =>
            {
                this.Actions.Add(new MethodViewModel(this) { Name = "Method" + ++methodCounter });
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

        ErrorViewModel structError;

        public StructViewModel()
        {
            structError = new ErrorViewModel() { Source = this };
            ErrorService.Instance.ObservableErrors.Add(structError);

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
                sb.AppendLine("Структура не может содержать двух одинаковых атрибутов");
            }
            if (!ValidateAction())
            {
                sb.AppendLine("Структура не может содержать двух одинаковых методов");
            }

            structError.Text = sb.ToString().TrimEnd();
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
            Attributes.CollectionChanged -= CollectionChanged;
            foreach (var item in Attributes)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            Actions.CollectionChanged -= CollectionChanged;
            foreach (var item in Actions)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            if (structError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(structError);
                structError = null;
            }
        }

    }
}
