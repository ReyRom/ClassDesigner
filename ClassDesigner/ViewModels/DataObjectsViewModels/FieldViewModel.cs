using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class FieldViewModel : ViewModelBase, IAttribute
    {
        private string name = "attribute";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

                OnPropertyChanged(nameof(AttributeString));
            }
        }

        private VisibilityType visibility = VisibilityType.Private;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        private TypeViewModel type = new TypeViewModel();
        public TypeViewModel TypeViewModel => type;
        public string Type
        {
            get => type.Type; set
            {
                type.Type = value;
                OnPropertyChanged(nameof(Type));

                OnPropertyChanged(nameof(AttributeString));
            }
        }

        private string defaultValue = string.Empty;
        public string DefaultValue
        {
            get => defaultValue; set
            {
                defaultValue = value;
                OnPropertyChanged(nameof(DefaultValue));

                OnPropertyChanged(nameof(AttributeString));
            }
        }

        private bool isStatic = false;

        public FieldViewModel(IEntry parent)
        {
            Parent = parent;

            type.PropertyChanged += Type_PropertyChanged;
        }

        private void Type_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Type));

            OnPropertyChanged(nameof(AttributeString));
        }

        public FieldViewModel(IEntry parent, FieldViewModel model) : this(parent)
        {
            Name = model.Name;
            Type = model.Type;
            DefaultValue = model.DefaultValue;
            Visibility = model.Visibility;
            IsStatic = model.IsStatic;
        }

        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;
                OnPropertyChanged(nameof(IsStatic));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        public string AttributeString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
                OnPropertyChanged(nameof(AttributeString));
            }
        }

        public IEntry Parent { get; set; }

        public static Match MatchAttributeString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)(\s:\s(?<Type>\w+)){0,1}(\s\=\s(?<DefV>\w+)){0,1}$");
        }

        public void ParseFromString(string data)
        {
            var m = MatchAttributeString(data);
            if (m.Success)
            {
                this.Name = m.Groups["Name"].Value;
                this.Type = m.Groups["Type"].Value;
                this.DefaultValue = m.Groups["DefV"].Value;
                this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
            }
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + (!string.IsNullOrWhiteSpace(Type) ? " : " + Type : "") + (DefaultValue != String.Empty ? " = " + DefaultValue : "");
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not FieldViewModel) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as FieldViewModel;
            return this.AttributeString == other.AttributeString
                && this.IsStatic == other.IsStatic;

        }
    }
}
