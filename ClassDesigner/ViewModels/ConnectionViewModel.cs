using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ConnectionViewModel:ViewModelBase
    {
        public ConnectionViewModel()
        {
            RelationType = SettingsService.Instance.RelationType;
        }

        public ClassViewModel SourceClass { get; set; }
        public ClassViewModel TargetClass { get; set;}
        public RelationType RelationType { get; set; }
        public ConnectionDataViewModel ConnectionData { get; set; }
    }
}
