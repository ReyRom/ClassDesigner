using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ClassStereotypes : ObservableCollection<StereotypeViewModel>
    {
        public ClassStereotypes()
        {
            this.Clear();
            this.Add(new StereotypeViewModel(Models.Stereotype.Struct, false));
            this.Add(new StereotypeViewModel(Models.Stereotype.Interface, false));
            this.Add(new StereotypeViewModel(Models.Stereotype.Enum, false));
        }
    }
}
