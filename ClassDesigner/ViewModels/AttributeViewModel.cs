using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AttributeViewModel : ViewModelBase
    {
        private string name = "attribute";
        private VisibilityType visibility = VisibilityType.Private;
        private string type = string.Empty;
        private bool isStatic = false;

        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        public VisibilityType Visibility
        {
            get => visibility; set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));

                OnPropertyChanged(nameof(AttributeString));
            }
        }
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));

                OnPropertyChanged(nameof(AttributeString));
            }
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

        public static Match MatchAttributeString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)(\s:\s(?<Type>\w+)){0,1}$");
        }

        public void ParseFromString(string data)
        {
            //валидация нужна

            var m = MatchAttributeString(data);
            this.Name = m.Groups["Name"].Value;
            this.Type = m.Groups["Type"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + (Type != String.Empty ? " : " + Type : "");
        }
    }
}
