﻿using ClassDesigner.Helping;
using ClassDesigner.Helping.Interfaces;
using ClassDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class ConstructorViewModel : ViewModelBase, IAction, IErrorProvider, IRelease
    {
        public ConstructorViewModel(IEntry parent)
        {
            Parent = parent;
            this.Name = Parent.Name;
            Parent.PropertyChanged += Parent_PropertyChanged; ;
            Parameters.CollectionChanged += Parameters_CollectionChanged;

            conctructorError = new ErrorViewModel() { Source = this, ErrorCriticalFor = ErrorCriticalFor.CodeGeneration };

            ErrorService.Instance.ObservableErrors.Add(conctructorError);
        }

        private void Parent_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                this.Name = Parent.Name;
            }
        }

        public ConstructorViewModel(IEntry parent, ConstructorViewModel model) : this(parent)
        {
            Name = model.Name;
            Type = model.Type;
            Visibility = model.Visibility;
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

            Validate();
        }

        private void Update(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ActionString));
            Validate();
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


        int parameterCounter = 0;

        private Command addParameterCommand;
        public Command AddParameterCommand => addParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Add(new ParameterViewModel() { Name = "param" + ++parameterCounter });
        });

        ErrorViewModel conctructorError;

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();

            if (!ValidateParameter())
            {
                sb.AppendLine("Определено несколько параметров с одинаковым именем");
            }

            conctructorError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateParameter()
        {
            var isValid = !Parameters.GroupBy(x => x.Name).Any(g => g.Count() > 1);
            return isValid;
        }


        private Command removeParameterCommand;


        public Command RemoveParameterCommand => removeParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Remove((ParameterViewModel)obj);
        });

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (obj.GetType() != this.GetType()) return false;

            var m = obj as ConstructorViewModel;
            return m.ActionString == this.ActionString;
        }
        public override string ToString()
        {
            return (char)Visibility + " " + Name + "(" + String.Join(", ", Parameters) + ")";
        }

        public void ParseFromString(string value)
        {
            var m = MatchMethodString(value);
            if (m.Success)
            {
                this.Name = m.Groups["Name"].Value;
                this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
                this.Parameters = ParseParameters(m.Groups["Parameters"].Value);
            }
        }
        public void ReleaseData()
        {
            if (conctructorError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(conctructorError);
                conctructorError = null;
            }
        }
    }
}

