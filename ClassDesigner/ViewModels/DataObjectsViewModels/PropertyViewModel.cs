using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class PropertyViewModel : ViewModelBase, IAttribute
    {
        public PropertyViewModel(IEntry parent)
        {
            Parent = parent;
        }

        public PropertyViewModel(IEntry parent, PropertyViewModel model) : this(parent)
        {
            Name = model.Name;
            Type = model.Type;
            DefaultValue = model.DefaultValue;
            Visibility = model.Visibility;
            IsStatic = model.IsStatic;
            IsAbstract = model.IsAbstract;
            IsGet = model.IsGet;
            IsSet = model.IsSet;
        }

        private string name = "Attribute";

        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        private VisibilityType visibility = VisibilityType.Public;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        private string type = string.Empty;
        public string Type
        {
            get => type; set
            {
                type = value;
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
        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;
                isAbstract = isStatic ? false : isAbstract;
                OnPropertyChanged(nameof(IsStatic));
                OnPropertyChanged(nameof(IsAbstract));
                OnPropertyChanged(nameof(AttributeString));
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
                OnPropertyChanged(nameof(AttributeString));
            }
        }

        private bool isGet = true;
        public bool IsGet
        {
            get => isGet; set
            {
                isGet = value;
                OnPropertyChanged(nameof(IsGet));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        private bool isSet = true;
        public bool IsSet
        {
            get => isSet; set
            {
                isSet = value;
                OnPropertyChanged(nameof(IsSet));

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

        public static Match MatchPropertyString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)(\s:\s(?<Type>\w+)){0,1}\s\{\s(?<Get>get;\s){0,1}(?<Set>set;\s){0,1}\}(\s\=\s(?<DefV>\w+)){0,1}$");
        }

        public void ParseFromString(string data)
        {
            //валидация нужна

            var m = MatchPropertyString(data);
            this.Name = m.Groups["Name"].Value;
            this.Type = m.Groups["Type"].Value;
            this.IsGet = !string.IsNullOrWhiteSpace(m.Groups["Get"].Value);
            this.IsSet = !string.IsNullOrWhiteSpace(m.Groups["Set"].Value);
            this.DefaultValue = m.Groups["DefV"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + (Type != String.Empty ? " : " + Type : "") + " { " + (IsGet ? "get; " : "") + (IsSet ? "set; " : "") + "}" + (DefaultValue != String.Empty ? " = " + DefaultValue : "");
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not PropertyViewModel) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as PropertyViewModel;
            return this.AttributeString == other.AttributeString
                && this.IsStatic == other.IsStatic && this.IsAbstract == other.IsAbstract;

        }
    }
}
