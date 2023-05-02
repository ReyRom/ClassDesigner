using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public interface IAction:INotifyPropertyChanged
    {
        string Name { get; set; }
        ObservableCollection<ParameterViewModel> Parameters { get; set; }
        VisibilityType Visibility { get; set; }
        string Type { get; set; }
    }
}
