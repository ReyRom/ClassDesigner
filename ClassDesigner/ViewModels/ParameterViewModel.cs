using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ParameterViewModel: ViewModelBase
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public string ParameterString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
            }
        }

        public static ParameterViewModel ParseFromString(string value)
        {
            ParameterViewModel parameter = new ParameterViewModel();
            var m = MatchParameterString(value);
            parameter.Name = m.Groups["Name"].Value;
            parameter.Type = m.Groups["Type"].Value;
            return parameter;
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
