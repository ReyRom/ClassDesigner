using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        public SettingsService SettingsService { get => SettingsService.Instance; }
    }
}
