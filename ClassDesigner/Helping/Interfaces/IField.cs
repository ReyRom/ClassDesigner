using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public interface IAttribute:INotifyPropertyChanged
    {
        string Name { get; set; }
        string Type { get; set; }

        VisibilityType Visibility { get; set; }
    }
}
