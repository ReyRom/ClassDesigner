using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public class PropertiesService : INotifyPropertyChanged
    {
        private static PropertiesService instance;
        private object selected;
        private object selectedCollection;

        public static PropertiesService Instance { get => instance ??= new PropertiesService(); }


        public object Selected
        {
            get => selected; set
            {
                selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public object SelectedCollection
        {
            get => selectedCollection; set
            {
                selectedCollection = value;
                OnPropertyChanged(nameof(SelectedCollection));
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
