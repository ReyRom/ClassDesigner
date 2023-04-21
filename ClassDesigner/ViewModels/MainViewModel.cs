using ClassDesigner.Helping;
using ClassDesigner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        public MainViewModel()
        {
            PropertiesWindow window = new PropertiesWindow();
            window.Show();
        }

        public SettingsService SettingsService { get => SettingsService.Instance; }
    }
}
