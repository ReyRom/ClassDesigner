using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public interface IHaveFields
    {
        ObservableCollection<IField> Attributes { get; set; }
        Command AddPropertyCommand { get; }
    }
}
