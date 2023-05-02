﻿using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    class ConstructorViewModel : ViewModelBase, IAction
    {
        public ConstructorViewModel(IEntry parent)
        {
            Parent = parent;
            Parameters.CollectionChanged += Parameters_CollectionChanged;
        }

        public ConstructorViewModel(IEntry parent, ConstructorViewModel model) : this(parent)
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

        private bool isStatic = false;
        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;

                IsAbstract = isStatic ? false : isAbstract;
                OnPropertyChanged(nameof(IsStatic));
                OnPropertyChanged(nameof(MethodString));
            }
        }

        private bool isAbstract = false;
        public bool IsAbstract
        {
            get => isAbstract; set
            {
                isAbstract = value;

                IsStatic = isAbstract ? false : isStatic;
                OnPropertyChanged(nameof(IsAbstract));

                OnPropertyChanged(nameof(MethodString));
            }
        }

        private string name = "Method";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(MethodString));
            }
        }

        private VisibilityType visibility = VisibilityType.Private;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
                OnPropertyChanged(nameof(MethodString));
            }
        }

        private string type;
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(MethodString));
            }
        }

        private ObservableCollection<ParameterViewModel> parameters = new ObservableCollection<ParameterViewModel>();
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

        public IEntry Parent { get; set; }

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
            return m.MethodString == this.MethodString && m.IsStatic == this.IsStatic;
        }
        public override string ToString()
        {
            return (char)Visibility + " " + Name + "(" + String.Join(", ", Parameters) + ")";
        }

        public void ParseFromString(string value)
        {
            var m = MatchMethodString(value);
            this.Name = m.Groups["Name"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
            this.Parameters = ParseParameters(m.Groups["Parameters"].Value);
        }
    }
}

