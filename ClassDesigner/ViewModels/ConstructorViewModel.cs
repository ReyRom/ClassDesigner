using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    class ConstructorViewModel:MethodViewModel
    {
        public override string ToString()
        {
            return (char)Visibility + " " + Name + "(" + String.Join(", ", Parameters) + ")";
        }

        public override void ParseFromString(string value)
        {
            var m = MatchMethodString(value);
            this.Name = m.Groups["Name"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
            this.Parameters = ParseParameters(m.Groups["Parameters"].Value);
        }
    }
}
