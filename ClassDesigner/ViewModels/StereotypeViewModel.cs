using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class StereotypeViewModel:ViewModelBase
    {
        public StereotypeViewModel(Stereotype stereotype, bool isSelected)
        {
            Stereotype = stereotype;
            IsSelected = isSelected;
        }

        public Stereotype Stereotype { get; set; }

        public bool IsSelected { get; set; }
    }
}
