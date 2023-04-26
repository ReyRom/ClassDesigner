using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class MethodViewModel : ViewModelBase, IMethod
    {
        public MethodViewModel()
        {
            Name = "Method";
            Visibility = VisibilityType.Private;
            Parameters = new ObservableCollection<ParameterViewModel>();
            Parameters.CollectionChanged += Parameters_CollectionChanged;

        }

        private void Parameters_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MethodString));
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= Update;
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += Update;
            }
        }

        private void Update(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MethodString));
        }

        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;
                OnPropertyChanged(nameof(IsStatic));
                OnPropertyChanged(nameof(MethodString));
            }
        }
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(MethodString));
            }
        }
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
                OnPropertyChanged(nameof(MethodString));
            }
        }
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(MethodString));
            }
        }
        public ObservableCollection<ParameterViewModel> Parameters
        {
            get => parameters; set
            {
                parameters = value;
                OnPropertyChanged(nameof(Parameters));
                OnPropertyChanged(nameof(MethodString));
            }
        }

        public virtual string MethodString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
                OnPropertyChanged(nameof(MethodString));
            }
        }
        public virtual void ParseFromString(string value)
        {
            var m = MatchMethodString(value);
            this.Name = m.Groups["Name"].Value;
            this.Type = m.Groups["Type"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
            this.Parameters = ParseParameters(m.Groups["Parameters"].Value);
        }

        protected virtual ObservableCollection<ParameterViewModel> ParseParameters(string value)
        {
            ObservableCollection<ParameterViewModel> parameters = new ObservableCollection<ParameterViewModel>();
            foreach (var item in value.Split(", "))
            {
                var param = new ParameterViewModel();
                param.ParseFromString(item);
                parameters.Add(param);
            }
            return parameters;
        }

        public virtual Match MatchMethodString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)([(](?<Parameters>(\w+(\s:\s\w+){0,1}(,\s\w+(\s:\s\w+))*){0,1})[)])(\s:\s(?<Type>\w+)){0,1}$");
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + "(" + String.Join(", ", Parameters) + ")" + (string.IsNullOrWhiteSpace(Type) ? "" : " : " + Type);
        }

        private Command addParameterCommand;
        public Command AddParameterCommand => addParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Add(new ParameterViewModel());
        });

        private Command removeParameterCommand;
        private string name = "Method";
        private VisibilityType visibility = VisibilityType.Private;
        private string type;
        private ObservableCollection<ParameterViewModel> parameters;
        private bool isStatic = false;

        public Command RemoveParameterCommand => removeParameterCommand ??= new Command(obj =>
                {
                    this.Parameters.Remove((ParameterViewModel)obj);
                });
    }
}
