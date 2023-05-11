using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class MethodViewModel : ViewModelBase, IAction
    {
        public MethodViewModel(IEntry parent)
        {
            Parent = parent;
            Parameters.CollectionChanged += Parameters_CollectionChanged;
        }

        public MethodViewModel(IEntry parent, MethodViewModel model) : this(parent)
        {
            Name = model.Name;
            Type = model.Type;
            Visibility = model.Visibility;
            IsStatic = model.IsStatic;
            IsAbstract = model.IsAbstract;
            Parameters = new ObservableCollection<ParameterViewModel>(model.Parameters);
        }

        private void Parameters_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ActionString));
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
            OnPropertyChanged(nameof(ActionString));
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
                OnPropertyChanged(nameof(ActionString));
            }
        }

        private bool isAbstract = false;
        public bool IsAbstract
        {
            get => isAbstract; set
            {
                isAbstract = value;

                isStatic = isAbstract ? false : isStatic;
                OnPropertyChanged(nameof(IsStatic));
                OnPropertyChanged(nameof(IsAbstract));
                OnPropertyChanged(nameof(ActionString));
            }
        }

        private string name = "Method";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(ActionString));
            }
        }

        private VisibilityType visibility = VisibilityType.Public;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
                OnPropertyChanged(nameof(ActionString));
            }
        }
        
        private string type;
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(ActionString));
            }
        }
        
        private ObservableCollection<ParameterViewModel> parameters = new ObservableCollection<ParameterViewModel>();
        public ObservableCollection<ParameterViewModel> Parameters
        {
            get => parameters; set
            {
                parameters = value;
                OnPropertyChanged(nameof(Parameters));
                OnPropertyChanged(nameof(ActionString));
            }
        }
        
        public virtual string ActionString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
                OnPropertyChanged(nameof(ActionString));
            }
        }

        public IEntry Parent { get; set; }

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
            if (string.IsNullOrWhiteSpace(value)) return parameters;
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
        

        public Command RemoveParameterCommand => removeParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Remove((ParameterViewModel)obj);
        });

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (obj.GetType() != this.GetType()) return false;

            var m = obj as MethodViewModel;
            return m.ActionString == this.ActionString && m.IsStatic == this.IsStatic;
        }
    }
}
