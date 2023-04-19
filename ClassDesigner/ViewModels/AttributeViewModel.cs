using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AttributeViewModel:ViewModelBase
    {
        public string Name { get; set; } = "attribute";
        public VisibilityType Visibility { get; set; } = VisibilityType.Private;
        public string Type { get; set; } = string.Empty;
        public bool IsStatic { get; set; } = false;
        public string AttributeString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
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
