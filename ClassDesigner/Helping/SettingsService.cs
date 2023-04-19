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
    public class SettingsService:INotifyPropertyChanged
    {
        private static SettingsService instance;
        private RelationType relationType = RelationType.Association;

        public static SettingsService Instance { get => instance ??= new SettingsService(); }
        public RelationType RelationType
        {
            get => relationType; set
            {
                relationType = value;
                OnPropertyChanged("RelationType");
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
