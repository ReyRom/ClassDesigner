using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AppSettingsViewModel:ViewModelBase
    {
        public double InterfaceScale
        {
            get
            {
                return Properties.Settings.Default.InterfaceScale;
            }
            set
            {
                Properties.Settings.Default.InterfaceScale = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(InterfaceScale));
            }
        }
    }
}
