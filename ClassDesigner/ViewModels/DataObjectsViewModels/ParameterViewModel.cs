using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ParameterViewModel : ViewModelBase
    {
        private string name = "name";
        private string type = "Type";

        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(ParameterString));
            }
        }
        public string Type
        {
            get => type; set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(ParameterString));
            }
        }

        private string defaultValue = string.Empty;
        private bool isOut = false;

        public string DefaultValue
        {
            get => defaultValue; set
            {
                defaultValue = value;
                OnPropertyChanged(nameof(DefaultValue));

                OnPropertyChanged(nameof(ParameterString));
            }
        }

        public string ParameterString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
                OnPropertyChanged(nameof(ParameterString));
            }
        }

        public bool IsOut { get => isOut; set => isOut = value; }
        public void ParseFromString(string value)
        {
            var m = MatchParameterString(value);
            if (m.Success) 
            {
                this.Name = m.Groups["Name"].Value;
                this.Type = m.Groups["Type"].Value;
            }
        }

        public static Match MatchParameterString(string value)
        {
            return Regex.Match(value, @"^(?<Name>\w+)(\s:\s(?<Type>\w+)){0,1}$");
        }

        public override string ToString()
        {
            return Name + (Type != String.Empty ? " : " + Type : "");
        }
    }
}
