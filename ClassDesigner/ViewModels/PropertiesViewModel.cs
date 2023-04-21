using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassDesigner.ViewModels
{
    public class PropertiesViewModel : ViewModelBase
    {
        public PropertiesViewModel()
        {
            PropertiesService.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Item));
        }

        public object Item { get => PropertiesService.Instance.Selected; }


    }
}
