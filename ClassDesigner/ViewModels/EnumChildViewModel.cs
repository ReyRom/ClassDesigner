using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ClassDesigner.ViewModels
{
    public class EnumChildViewModel : ViewModelBase
    {
        public string Name { get; set; } = "value";
        public string Value { get; set; }

        public string EnumString
        { 
            get => ToString(); 
            set => ParseFromString(value); 
        }

        public static Match MatchEnumString(string value)
        {
            return Regex.Match(value, @"^(?<Name>\w+)(\s\=\s(?<Value>\w+)){0,1}$");
        }

        public void ParseFromString(string data)
        {
            //валидация нужна

            var m = MatchEnumString(data);
            this.Name = m.Groups["Name"].Value;
            this.Value = m.Groups["Value"].Value;
        }

        public override string ToString()
        {
            return Name + (Value != String.Empty ? " = " + Value : "");
        }
    }
}
