using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AttributeViewModel : ViewModelBase, IField
    {
        private string name = "attribute";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

                OnPropertyChanged(nameof(FieldString));
            }
        }

        private VisibilityType visibility = VisibilityType.Private;
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));

                OnPropertyChanged(nameof(FieldString));
            }
        }
        private string type = string.Empty;
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));

                OnPropertyChanged(nameof(FieldString));
            }
        }

        private string defaultValue = string.Empty;
        public string DefaultValue
        {
            get => defaultValue; set
            {
                defaultValue = value;
                OnPropertyChanged(nameof(DefaultValue));

                OnPropertyChanged(nameof(FieldString));
            }
        }

        private bool isStatic = false;
        public bool IsStatic
        {
            get => isStatic; set
            {
                isStatic = value;
                OnPropertyChanged(nameof(IsStatic));

                OnPropertyChanged(nameof(FieldString));
            }
        }
        public string FieldString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
                OnPropertyChanged(nameof(FieldString));
            }
        }

        public IEntry Parent { get; set; }

        public static Match MatchAttributeString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)(\s:\s(?<Type>\w+)){0,1}(\s\=\s(?<DefV>\w+)){0,1}$");
        }

        public void ParseFromString(string data)
        {
            //валидация нужна

            var m = MatchAttributeString(data);
            this.Name = m.Groups["Name"].Value;
            this.Type = m.Groups["Type"].Value;
            this.DefaultValue = m.Groups["DefV"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + (Type != String.Empty ? " : " + Type : "") + (DefaultValue != String.Empty ? " = " + DefaultValue : "");
        }
    }
}
