using ClassDesigner.Helping.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public interface IEntry : INotifyPropertyChanged, IRelease
    {
        public string Name { get; set; }
    }
}
